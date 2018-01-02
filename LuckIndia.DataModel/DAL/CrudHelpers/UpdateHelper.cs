using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using LuckIndia.DataModel.DAL.CrudHelpers;
using LuckIndia.Models;
using LuckIndia.DataModel;
using Alphaeon.Services.EnterpriseAPI.DAL.CachingServices;
using LuckIndia.Models.Interfaces;
using LuckIndia.DataModel.DAL.Enums;
using LuckIndia.APIs.DAL.Exceptions;
using LuckIndia.DataModel.DAL.CachingServices;
using System.IO;
using LuckIndia.DataModel.DAL.Attributes;

namespace Alphaeon.Services.EnterpriseAPI.DAL.CrudHelpers
{
    sealed class UpdateHelper<T> : CrudHelper<T> where T : Model
    {
        private readonly IDictionary<string, object> _delta;
        private readonly IDictionary<string, object> _oldValues = new Dictionary<string, object>();
        private readonly IDictionary<string, object> _newValues = new Dictionary<string, object>();

        public UpdateHelper(T model, IDictionary<string, object> delta, LuckIndiaDBContext context, bool shouldValidatePermissions = true)
            : base(model, context, shouldValidatePermissions)
        {
            _delta = delta;
        }

        protected override void ValidateApplication()
        {
            //var applicationId = this.Context.GetCurrentApplicationId();

            //var valid = ApplicationPermissionCachingService.GetCurrent().CanUpdate(applicationId, this.ModelClassId);

            //if (!valid)
            //{
            //    throw new ForbiddenException("Application does not have permission. For model class id : " + this.ModelClassId, ExceptionSeverity.Information);
            //}
        }

        protected override void ValidateUser()
        {
            var userId = this.Context.GetCurrentUser().Id;

            var valid = RolePermissionCachingService.GetCurrent().CanUpdate(this.ModelClassId, Context);

            if (!valid)
            {
                throw new ForbiddenException("No permission. For model class id : " + this.ModelClassId, ExceptionSeverity.Information);
            }
        }

        //protected override IEnumerable<IValidatable> GetDataRules()
        //{
        //    return DataRulesCachingService.GetCurrent().GetUpdateRules(this.Model, _delta);
        //}

        //protected override IEnumerable<IValidatable> GetBusinessRules()
        //{
        //    return BusinessRulesCachingService.GetCurrent().GetUpdateRules(this.Model, _delta, this.Context);
        //}

        //protected override IEnumerable<IExecutable> GetCrudRules()
        //{
        //    return CrudRulesCachingService.GetCurrent().GetUpdateRules(this.Model, _oldValues, this.Context);
        //}

        //protected override IEnumerable<CrudHook<T>> GetCrudHooks()
        //{
        //    return CrudHooksCachingService.GetCurrent().GetUpdateHooks(this.Model, _oldValues, this.Context);
        //}

        protected override void ExecuteCrudOperation()
        {
            this.Context.Logger.AppendLine("ExecuteCrudOperation Started");
            var sw = Stopwatch.StartNew();

            var allProperties = GetProperties().ToList();
            var nonPatchablePropertyNames = GetNonPatchablePropertyNames().ToList();

            var validationErrors = new StringBuilder();

            foreach (var prop in _delta)
            {
                var key = prop.Key;
                var propToSet = allProperties.FirstOrDefault(x => x.Name == key);

                if (propToSet == null)
                {
                    validationErrors.AppendFormat("Invalid property: {0}", key);
                    validationErrors.AppendLine();
                    continue;
                }

                if (this.ShouldValidatePermissions && nonPatchablePropertyNames.Contains(key))
                {
                    validationErrors.AppendFormat("Read-only property: {0}", key);
                    validationErrors.AppendLine();
                    continue;
                }

                try
                {
                    //get the old value
                    _oldValues.Add(key, propToSet.GetValue(this.Model));

                    propToSet.SetValue(this.Model, prop.Value);

                    //in case the model alters the value at all during a setter, pull it fresh.
                    _newValues.Add(key, propToSet.GetValue(this.Model));
                }
                catch (Exception ex)
                {
                    var message = ex.InnerException == null ? ex.Message : ex.InnerException.Message;
                    validationErrors.AppendFormat("Error updating {0}: {1}", prop.Key, message);
                    validationErrors.AppendLine();
                }
            }

            if (validationErrors.Length > 0)
            {
                throw new InvalidDataException(validationErrors.ToString());
            }

            this.Context.SaveChanges();

            sw.Stop();
            this.Context.Logger.AppendLine(string.Format("ExecuteCrudOperation Done: {0} ms", sw.ElapsedMilliseconds));
        }

        //protected override void ExecuteModelEventLogging()
        //{
        //    if (typeof(T) == typeof(ModelEvent))
        //    {
        //        return;
        //    }

        //    this.Context.Logger.AppendLine("ExecuteModelEventLogging Started");
        //    var sw = Stopwatch.StartNew();

        //    var jsonBefore = JsonConvert.SerializeObject(_oldValues);
        //    var jsonAfter = JsonConvert.SerializeObject(_newValues);

        //    var modelEvent = new ModelEvent
        //    {
        //        ModelEventTypeId = (int)ModelEventTypeValue.Updated,
        //        ModelClassId = this.ModelClassId,
        //        ModelId = this.Model.Id,
        //        JsonBefore = jsonBefore,
        //        JsonAfter = jsonAfter
        //    };


        //    try
        //    {
        //        this.Context.Create(modelEvent, false);
        //    }
        //    catch (Exception ex)
        //    {
        //        DAL.LogException(ex, LogCategory.ModelEventUpdate);
        //    }

        //    sw.Stop();
        //    this.Context.Logger.AppendLine(string.Format("ExecuteModelEventLogging Done: {0} ms", sw.ElapsedMilliseconds));
        //}



        //todo: move this to a property caching service

        private static IEnumerable<PropertyInfo> _properties;
        private static IEnumerable<PropertyInfo> GetProperties()
        {
            if (_properties == null)
            {
                _properties = typeof(T).GetProperties().Where(x => x.CanWrite);
            }

            return _properties;
        }

        private static IEnumerable<string> _nonPatchablePropertyNames;
        private static IEnumerable<string> GetNonPatchablePropertyNames()
        {
            if (_nonPatchablePropertyNames == null)
            {
                _nonPatchablePropertyNames = GetProperties()
                    .Where(x => System.Attribute.IsDefined(x, typeof(NonPatchableAttribute)))
                    .Select(x => x.Name);
            }

            return _nonPatchablePropertyNames;
        }
    }
}
