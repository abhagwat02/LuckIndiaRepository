using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Web.Http.Description;
using LuckIndia.DataModel;
using LuckIndia.Models;
using LuckIndia.Models.Interfaces;
using LuckIndia.APIs.Factories;
using System.Web.Http.OData.Query;
using System.Web.Http.OData.Extensions;
using LuckIndia.APIs.DataInjectors;

namespace LuckIndia.APIs.Controllers
{

    public abstract class RestApiController<TModel, TDTO> : AuthenticatedApiController
        where TModel : Model
        where TDTO : IModelDTO
    {
        protected readonly Factory<TModel, TDTO> Factory = FactoryFactory.GetFactory<TModel, TDTO>();

        [NonAction]
        protected virtual IQueryable<TModel> GetCollection(ODataQueryOptions<TModel> queryOptions)
        {
            using (var context = LuckIndiaDBContext.GetContextWithAccessToken(this.GetAccessToken(), this.GetLogger()))
            {
                var collection = context.GetCollection<TModel>();

                this.GetLogger().AppendLine("OData ApplyTo Started");

                var sw = Stopwatch.StartNew();

                collection = (IQueryable<TModel>)queryOptions.ApplyTo(collection, new ODataQuerySettings { PageSize = WebApiConfig.PageSize });

                sw.Stop();
                this.GetLogger().AppendLine(string.Format("OData ApplyTo Done: {0} ms", sw.ElapsedMilliseconds));
                
                return collection;
            }
        }


        /// <summary>
        /// Gets a collection with the given odata
        /// </summary>
        public virtual IHttpActionResult Get(ODataQueryOptions<TModel> queryOptions)
        {
            Stopwatch sw = Stopwatch.StartNew();
            var metadata = "{}";
            var collection = this.GetCollection(queryOptions);

            if (Request.ODataProperties().TotalCount.HasValue)
            {
                this.GetLogger().AppendLine("TotalCount Started");
                sw = Stopwatch.StartNew();

                var pagerMetadata = new
                {
                    count = Request.ODataProperties().TotalCount,
                    maxPageSize = WebApiConfig.PageSize
                };

                metadata = JsonConvert.SerializeObject(pagerMetadata, WebApiConfig.StandardJsonSerializerSettings);

                sw.Stop();
                this.GetLogger().AppendLine(string.Format("TotalCount Done: {0} ms", sw.ElapsedMilliseconds));
            }

            this.GetLogger().AppendLine("ToDTOs started");
            var returnSet = Factory.ToDTOs(collection.ToList());
            sw.Stop();
            this.GetLogger().AppendLine(string.Format("ToDTOs done: {0} ms", sw.ElapsedMilliseconds));

            return this.Ok(returnSet, metadata);
        }

        /// <summary>
        /// Gets a single model.
        /// </summary>
        /// <param name="id">Id of the entity.</param>
        public virtual async Task<HttpResponseMessage> Get(int? id)
        {
            //nullable needed incase something other than an int is passed.
            if (!id.HasValue)
            {
                return await this.NotFound().ExecuteAsync(CancellationToken.None);
            }

            //var acceptTypes = this.GetAcceptContentTypes();

            TDTO dto;

            using (var context = LuckIndiaDBContext.GetContextWithAccessToken(this.GetAccessToken(), this.GetLogger()))
            {
                var model = context.GetSingle<TModel>(id.Value);

                //if (acceptTypes.Any() && !acceptTypes.Contains(MediaTypeValue.ANY) && !acceptTypes.Contains(MediaTypeValue.JSON))
                //{
                  // return await this.PerformContentNegotiation(model, acceptTypes);
                //}

                //default functionality is to return the dto.
                dto = this.Factory.ToDTO(model);
            }

            return await this.Ok(dto).ExecuteAsync(CancellationToken.None);
        }

        /// <summary>
        /// Converts the given model to a dto and responds to the client. All permissions and error checking has been done before this method is called.
        /// You would want to override this if you want to do content negotiation and respond with something other than JSON.
        /// </summary>
        [NonAction]
        public virtual async Task<HttpResponseMessage> PerformContentNegotiation(TModel model, List<string> acceptTypes)
        {
            return await Task.FromResult(this.Request.CreateErrorResponse(HttpStatusCode.UnsupportedMediaType,
                "Unsupported media type. Try application/json instead."));
        }

        /// <summary>
        /// Overriders must call this.Init() and run through permission checks and fire all the events.
        /// Do not call base. Typically only overridden to completely disable this method by using [NonAction] and then implementing your own Post()
        /// </summary>
        /// <param name="postedDto"></param>
        /// <returns></returns>
        public virtual IHttpActionResult Post(TDTO postedDto)
        {
            if (postedDto == null)
            {
                return this.BadRequest("Invalid DTO.");
            }

            TModel model;

            try
            {
                model = this.Factory.FromDTO(postedDto);
            }
            catch (Exception ex)
            {
                return this.BadRequest(ex.Message);
            }

            using (var context = LuckIndiaDBContext.GetContextWithAccessToken(this.GetAccessToken(), this.GetLogger()))
            {
                model = context.Create(model);
                return this.Created(this.GetLocationUri(model), this.Factory.ToDTO(model));
            }
        }

        /// <summary>
        /// Patches a model with the given changes.
        /// </summary>
        /// <param name="id">Id of the model.</param>
        /// <param name="json">Json object of the changes. Needs to be a single object with properties of base data types.</param>
        /// <returns></returns>
        public virtual IHttpActionResult Patch(int? id, JObject json)
        {
            //nullable needed incase something other than an int is passed.
            if (!id.HasValue)
            {
                return this.NotFound();
            }

            if (json == null)
            {
                return this.BadRequest("Invalid JSON.");
            }

            using (var context = LuckIndiaDBContext.GetContextWithAccessToken(this.GetAccessToken(), this.GetLogger()))
            {
                 var userId = context.GetCurrentUser().Id;

                var dataInjectors = new List<IDataInjectable>
                    {
                        new MyUserIdInjector(userId),
                        new UtcNowInjector()
                    };

                var allProperties = typeof(TModel).GetProperties().Where(x => x.CanWrite).ToList();

                var validationErrors = new StringBuilder();

                var delta = new Dictionary<string, object>();
                
                var modelFound = context.Set<TModel>().FirstOrDefault(x => x.Id == id);
                if (modelFound == null)
                {
                   // CMDLogger.LogBusinessException("Model ID not found : " + id, json, string.Empty);                   
                    
                }
                foreach (var o in json)
                {
                    var propInfo = allProperties.FirstOrDefault(x => String.Equals(x.Name, o.Key, StringComparison.CurrentCultureIgnoreCase));
                    if (null == propInfo)
                    {
                        continue;
                    }

                    var type = propInfo.PropertyType;

                    var injected = false;
                    foreach (var injectable in dataInjectors)
                    {
                        object tmp;

                        if (!injectable.TryParseObject(type, o.Value.ToString(), out tmp))
                        {
                            continue;
                        }

                        delta.Add(propInfo.Name, tmp);
                        injected = true;
                        break;
                    }

                    if (injected)
                    {
                        continue;
                    }

                    try
                    {
                        delta.Add(propInfo.Name, o.Value.ToObject(type));
                    }
                    catch
                    {
                        validationErrors.AppendFormat("Invalid data for: {0}", o.Key);
                        validationErrors.AppendLine();
                    }
                }

                if (json.Count != delta.Count)
                {
                    validationErrors.AppendLine("Invalid properties.");
                }

                if (validationErrors.Length > 0)
                {
                    return this.BadRequest(validationErrors.ToString().Trim());
                }

                var model = context.Update<TModel>(id.Value, delta);

                return this.Ok(this.Factory.ToDTO(model));
            }
        }

        /// <summary>
        /// Deletes the given model.
        /// </summary>
        /// <param name="id">Id of the model.</param>
        public virtual IHttpActionResult Delete(int? id)
        {
            //nullable needed incase something other than an int is passed.
            if (!id.HasValue)
            {
                return this.NotFound();
            }

            using (var context = LuckIndiaDBContext.GetContextWithAccessToken(this.GetAccessToken(), this.GetLogger()))
            {
                context.Delete<TModel>(id.Value);
            }

            return this.OkNoContent();
        }


        /// <summary>
        /// The route needed to fill the location header when a new entity is created.
        /// The format will be /[LocationUri]/{id}. Ex: If LocationUri is set to Users the result will be /Users/1.
        /// </summary>
        protected Uri GetLocationUri(TModel model)
        {
            var routeData = this.Request.GetRouteData();
            string pluralModelName = null;
            if (routeData.Values.ContainsKey(WebApiConfig.ROUTE_PARAM_CONTROLLER))
            {
                pluralModelName = routeData.Values[WebApiConfig.ROUTE_PARAM_CONTROLLER].ToString();
            }

            if (null == pluralModelName)
            {
                return new Uri("unknown", UriKind.Relative);
            }

            return new Uri(string.Format("/{0}/{1}", pluralModelName, model.Id), UriKind.Relative);
        }


       // private List<string> _acceptContentType;

        /// <summary>
        /// Returns a list of strings that are the accept types requested.
        /// This will check file extension first if present, then look at the accept header.
        /// </summary>
        //[NonAction]
        //private List<string> GetAcceptContentTypes()
        //{
        //    //cache the list so any subclasses don't need to generate this list every time.
        //    if (_acceptContentType != null)
        //    {
        //        return _acceptContentType;
        //    }

        //    _acceptContentType = new List<string>();

        //    string ext = null;
        //    var routeData = this.Request.GetRouteData();
        //    if (routeData.Values.ContainsKey("ext"))
        //    {
        //        ext = routeData.Values["ext"].ToString().ToLower();
        //    }

        //    if (ext == null)
        //    {
        //        //get accept types from Accept header
        //        _acceptContentType = this.Request.Headers.Accept.Select(x => x.MediaType).ToList();
        //    }
        //    else
        //    {
        //        var getContentTypesRunnable = new GetContentTypesFromFileExtension(ext);

        //        using (var context = CMDDatabaseContext.GetContext(this.GetLogger()))
        //        {
        //            _acceptContentType = context.ExecuteRunnable<List<string>>(getContentTypesRunnable);
        //        }
        //    }

        //    return _acceptContentType;
        //}
    }
}
