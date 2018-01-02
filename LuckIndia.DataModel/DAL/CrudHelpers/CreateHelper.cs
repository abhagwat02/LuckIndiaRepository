using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.CachingServices;
using Newtonsoft.Json;
using LuckIndia.DataModel.DAL.CrudHelpers;
using LuckIndia.Models;
using LuckIndia.DataModel;
using LuckIndia.Models.Interfaces;
using LuckIndia.DataModel.DAL.CachingServices;
using LuckIndia.DataModel.Interfaces;

namespace Alphaeon.Services.EnterpriseAPI.DAL.CrudHelpers
{
    sealed class CreateHelper<T> : CrudHelper<T> where T : Model
    {
        public CreateHelper(T model, LuckIndiaDBContext context, bool shouldValidatePermissions = true) : base(model, context, shouldValidatePermissions) { }

        protected override void ValidateApplication()
        {
            //var applicationId = this.Context.GetCurrentApplicationId();

            //var valid = ApplicationPermissionCachingService.GetCurrent().CanCreate(applicationId, this.ModelClassId);

            //if (!valid)
            //{
            //    throw new ForbiddenException("Application does not have permission. For model class id : " + this.ModelClassId, ExceptionSeverity.Information);
            //}
        }

        protected override void ValidateUser()
        {
            var userId = this.Context.GetCurrentUser().Id;

            var valid = RolePermissionCachingService.GetCurrent().CanCreate(this.ModelClassId, Context);

            if (!valid)
            {
                //throw new ForbiddenException("No permission. For model class id : " + this.ModelClassId, ExceptionSeverity.Information);
            }
        }

        protected override void ValidateCollection()
        {
            //Nothing to validate because we are creating the model.
        }

        //protected override IEnumerable<IValidatable> GetDataRules()
        //{
        //    return DataRulesCachingService.GetCurrent().GetCreateRules(this.Model);
        //}

        //protected override IEnumerable<IValidatable> GetBusinessRules()
        //{
        //    return BusinessRulesCachingService.GetCurrent().GetCreateRules(this.Model, this.Context);
        //}

        protected override IEnumerable<IExecutable> GetCrudRules()
        {
            return CrudRulesCachingService.GetCurrent().GetCreateRules(this.Model, this.Context);
        }

        //protected override IEnumerable<CrudHook<T>> GetCrudHooks()
        //{
        //    return CrudHooksCachingService.GetCurrent().GetCreateHooks(this.Model, this.Context);
        //}

        protected override void ExecuteCrudOperation()
        {
           try
           {

                this.Context.Logger.AppendLine("ExecuteCrudOperation Started");
                var sw = Stopwatch.StartNew();

                this.Context.Set<T>().Add(this.Model);
                this.Context.SaveChanges();

                sw.Stop();
                this.Context.Logger.AppendLine(string.Format("ExecuteCrudOperation Done: {0} ms", sw.ElapsedMilliseconds));
             }
           catch (System.Data.Entity.Validation.DbEntityValidationException ex)
           {
               //DAL.LogException(ex, LogCategory.ModelEventCreate);
                   
             }
        }

        protected override void ExecuteModelEventLogging()
        {
            //if (typeof(T) == typeof(ModelEvent))
            //{
            //    return;
            //}

            //this.Context.Logger.AppendLine("ExecuteModelEventLogging Started");
            //var sw = Stopwatch.StartNew();

            //var model = this.Context.Set<T>().FirstOrDefault(x => x.Id == this.Model.Id);

            //this.Context.Detach(model);

            //var jsonAfter = JsonConvert.SerializeObject(model);

            //var modelEvent = new ModelEvent
            //{
            //    ModelEventTypeId = (int)ModelEventTypeValue.Created,
            //    ModelClassId = this.ModelClassId,
            //    ModelId = this.Model.Id,
            //    JsonAfter = jsonAfter,
            //};


            //try
            //{
            //    this.Context.Create(modelEvent, false);
            //}
            //catch (Exception ex)
            //{
            //    DAL.LogException(ex, LogCategory.ModelEventCreate);
            //}

            //sw.Stop();
            //this.Context.Logger.AppendLine(string.Format("ExecuteModelEventLogging Done: {0} ms", sw.ElapsedMilliseconds));
        }
    }
}
