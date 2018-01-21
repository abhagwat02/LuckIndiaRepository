using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Web.Http;

using Newtonsoft.Json;

using LuckIndia.APIs.HttpResponses;
using LuckIndia.DataModel.Authorizations;
using LuckIndia.DataModel.Interfaces;
using LuckIndia.DataModel.LoggingServices;
using System.Web;

namespace LuckIndia.APIs.Controllers

{
    public abstract class BaseApiController : ApiController
    {
        private NameValueCollection _queryParams;
        private ILogger _logger;

        /// <summary>
        /// This will attempt to get the AccessToken from the request.
        /// This will be null if an invalid AccessToken was passed with the request.
        /// </summary>
        /// <returns></returns>
        [NonAction]
        protected SecurityContext GetAccessToken()
        {
            var principal = this.GetApiPrincipal();
            return principal == null ? null : principal.securityContext;
        }

        /// <summary>
        /// This will attempt to cast an ApiPrincipal from the this.User property.
        /// This will be null if an invalid AccessToken was passed with the request.
        /// </summary>
        /// <returns></returns>
        [NonAction]
        protected ApiPrincipal GetApiPrincipal()
        {
            return this.User as ApiPrincipal;
        }


        /// <summary>
        /// Attempts to get the X-Metadata header as json and parse it to the given type.
        /// </summary>
        /// <typeparam name="T">Type of object to parse to</typeparam>
        [NonAction]
        protected T GetMetadataHeader<T>()
        {
            IEnumerable<string> headerCollection;

            if (!this.Request.Headers.TryGetValues("X-Metadata", out headerCollection))
            {
                return default(T);
            }

            var headerItems = headerCollection.ToList();

            if (headerItems.Count() != 1)
            {
                return default(T);
            }

            var meta = headerItems.FirstOrDefault();

            return !string.IsNullOrWhiteSpace(meta) ? JsonConvert.DeserializeObject<T>(meta) : default(T);
        }

        /// <summary>
        /// Gets a query string parameter for the given key.
        /// Returns null if the key is not found.
        /// </summary>
        /// <param name="key"></param>
        [NonAction]
        protected string GetQuerystringParameter(string key)
        {
            if (_queryParams == null)
            {
                _queryParams = this.Request.RequestUri.ParseQueryString();
            }

            return _queryParams[key];
        }

        protected ILogger GetLogger()
        {
            if (_logger == null)
            {
                _logger = HttpContext.Current.Items[WebApiConfig.LoggerHttpContextItemsKey] as ILogger ?? new FakeLoggerService();
            }

            return _logger;
        }

        #region Custom Responses

        /// <summary>
        /// Returns an Ok response with the given string placed in the X-Metadata header.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <param name="metadata"></param>
        /// <returns></returns>
        protected OkNegotiatedContentResultWithMetadata<T> Ok<T>(T content, string metadata)
        {
            return new OkNegotiatedContentResultWithMetadata<T>(content, metadata, this);
        }

        /// <summary>
        /// Returns an Ok No Content response.
        /// </summary>
        /// <returns></returns>
        protected OkNoContentResult OkNoContent()
        {
            return new OkNoContentResult(this);
        }

        /// <summary>
        /// Returns an Ok Accepted response.
        /// </summary>
        /// <returns></returns>
        protected OkAcceptedResult OkAccepted()
        {
            return new OkAcceptedResult(this);
        }

        #endregion
    }
}
