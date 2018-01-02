using System;
using System.Collections.Generic;
using System.Diagnostics;
using Alphaeon.Services.EnterpriseAPI.DAL.CachingServices;
using Newtonsoft.Json;
using LuckIndia.Models;

namespace LuckIndia.DataModel.DAL.CrudHelpers
{
    sealed class DeleteHelper<T> : CrudHelper<T> where T : Model
    {
        public DeleteHelper(T model, LuckIndiaDBContext context, bool shouldValidatePermissions = true) : base(model, context, shouldValidatePermissions) { }

        protected override void ValidateApplication()
        {
            //var applicationId = this.Context.GetCurrentApplicationId();

            //var valid = ApplicationPermissionCachingService.GetCurrent().CanDelete(applicationId, this.ModelClassId);

            //if (!valid)
            //{
            //    throw new ForbiddenException("Application does not have permission. For model class id : " + this.ModelClassId, ExceptionSeverity.Information);
            //}
        }

        protected override void ValidateUser()
        {
            //var userId = this.Context.GetCurrentUser().Id;

            //var valid = RolePermissionCachingService.GetCurrent().CanDelete(this.ModelClassId, Context);

            //if (!valid)
            //{
            //    throw new ForbiddenException("No permission. For model class id : " + this.ModelClassId, ExceptionSeverity.Information);
            //}
        }

        //protected override IEnumerable<IValidatable> GetDataRules()
        //{
        //    return DataRulesCachingService.GetCurrent().GetDeleteRules(this.Model);
        //}

        //protected override IEnumerable<IValidatable> GetBusinessRules()
        //{
        //    return BusinessRulesCachingService.GetCurrent().GetDeleteRules(this.Model, this.Context);
        //}

        //protected override IEnumerable<IExecutable> GetCrudRules()
        //{
        //    return CrudRulesCachingService.GetCurrent().GetDeleteRules(this.Model, this.Context);
        //}

        //protected override IEnumerable<CrudHook<T>> GetCrudHooks()
        //{
        //    return CrudHooksCachingService.GetCurrent().GetDeleteHooks(this.Model, this.Context);
        //}

        protected override void ExecuteCrudOperation()
        {
            //if (this.ShouldValidatePermissions && typeof(T).IsDefined(typeof(NonDeletableAttribute), true))
            //{
            //    throw new ForbiddenException("Deleting this type of object is forbidden.");
            //}

            this.Context.Logger.AppendLine("ExecuteCrudOperation Started");
            var sw = Stopwatch.StartNew();

            this.Context.Set<T>().Remove(this.Model);
            this.Context.SaveChanges();

            sw.Stop();
            this.Context.Logger.AppendLine(string.Format("ExecuteCrudOperation Done: {0} ms", sw.ElapsedMilliseconds));
        }

        protected override void ExecuteModelEventLogging()
        {
            //if (typeof(T) == typeof(ModelEvent))
            //{
            //    return;
            //}

            //this.Context.Logger.AppendLine("ExecuteModelEventLogging Started");
            //var sw = Stopwatch.StartNew();

            //var jsonBefore = JsonConvert.SerializeObject(this.Model);

            //var modelEvent = new ModelEvent
            //{
            //    ModelEventTypeId = (int)ModelEventTypeValue.Deleted,
            //    ModelClassId = this.ModelClassId,
            //    ModelId = this.Model.Id,
            //    JsonBefore = jsonBefore
            //};


            //try
            //{
            //    this.Context.Create(modelEvent, false);
            //}
            //catch (Exception ex)
            //{
            //    DAL.LogException(ex, LogCategory.ModelEventDelete);
            //}

            //sw.Stop();
            //this.Context.Logger.AppendLine(string.Format("ExecuteModelEventLogging Done: {0} ms", sw.ElapsedMilliseconds));
        }
    }
}
