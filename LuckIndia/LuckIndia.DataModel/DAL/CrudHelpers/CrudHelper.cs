using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LuckIndia.Models;
using LuckIndia.Models.Interfaces;
using Alphaeon.Services.EnterpriseAPI.DAL.CachingServices;
using LuckIndia.DataModel.Interfaces;
using LuckIndia.APIs.DAL.Exceptions;
using LuckIndia.DataModel.DAL.CachingServices;

namespace LuckIndia.DataModel.DAL.CrudHelpers
{
    abstract class CrudHelper<T> where T : Model
    {
        protected T Model;
        protected readonly int ModelClassId;
        protected readonly LuckIndiaDBContext Context;
        protected readonly bool ShouldValidatePermissions;

        protected CrudHelper(T model, LuckIndiaDBContext context, bool shouldValidatePermissions = true)
        {
            this.Model = model;
            this.Context = context;
            this.ShouldValidatePermissions = shouldValidatePermissions;
            this.ModelClassId = ModelClassCachingService.GetCurrent().GetModelClassId<T>();
        }

        protected virtual IEnumerable<IValidatable> GetDataRules()
        {
            return new List<IValidatable>();
        }

        protected virtual IEnumerable<IValidatable> GetBusinessRules()
        {
            return new List<IValidatable>();
        }

        protected virtual IEnumerable<IExecutable> GetCrudRules()
        {
            return new List<IExecutable>();
        }

        //protected virtual IEnumerable<CrudHook<T>> GetCrudHooks()
        //{
        //    return new List<CrudHook<T>>();
        //}

        public void Execute()
        {
            ValidatePermissions();

            //ValidateDataRules();

            //ValidateBusinessRules();

            StartExecuteCrudOperation();

           // StartExecuteModelEventLogging();

            ExecuteCrudRules();

            //ExecuteCrudHooks();
        }


        private void ValidatePermissions()
        {
            if (!ShouldValidatePermissions)
            {
                this.Context.Logger.AppendLine("ValidateApplication Skipped");
                return;
            }

            this.Context.Logger.AppendLine("ValidateApplication Started");
            var sw = Stopwatch.StartNew();

            try
            {
                ValidateApplication();
            }
            catch (Exception ex)
            {
              //  DAL.LogException(ex, LogCategory.ApplicationValidation);
                throw;
            }

            sw.Stop();
            this.Context.Logger.AppendLine(string.Format("ValidateApplication Done: {0} ms", sw.ElapsedMilliseconds));


            this.Context.Logger.AppendLine("ValidateUser Started");
            sw = Stopwatch.StartNew();

            try
            {
                ValidateUser();
            }
            catch (Exception ex)
            {
               // DAL.LogException(ex, LogCategory.UserValidation);
                throw;
            }

            sw.Stop();
            this.Context.Logger.AppendLine(string.Format("ValidateUser Done: {0} ms", sw.ElapsedMilliseconds));


            this.Context.Logger.AppendLine("ValidateCollection Started");
            sw = Stopwatch.StartNew();

            ValidateCollection();

            sw.Stop();
            this.Context.Logger.AppendLine(string.Format("ValidateCollection Done: {0} ms", sw.ElapsedMilliseconds));
        }

        private void ValidateDataRules()
        {
            this.Context.Logger.AppendLine("ValidateDataRules Started");
            var sw = Stopwatch.StartNew();

            try
            {
                foreach (var rule in this.GetDataRules())
                {
                    this.Context.Logger.AppendLine(string.Format("Validating {0}", rule));

                    rule.Validate();
                }
            }
            catch (Exception ex)
            {
                //DAL.LogException(ex, LogCategory.DataRule);
                throw;
            }

            sw.Stop();
            this.Context.Logger.AppendLine(string.Format("ValidateDataRules Done: {0} ms", sw.ElapsedMilliseconds));
        }

        private void ValidateBusinessRules()
        {
            if (!ShouldValidatePermissions)
            {
                this.Context.Logger.AppendLine("ValidateBusinessRules Skipped");
                return;
            }

            this.Context.Logger.AppendLine("ValidateBusinessRules Started");
            var sw = Stopwatch.StartNew();

            try
            {
                foreach (var rule in this.GetBusinessRules())
                {
                    this.Context.Logger.AppendLine(string.Format("Validating {0}", rule));

                    rule.Validate();
                }
            }
            catch (Exception ex)
            {
               // DAL.LogException(ex, LogCategory.BusinessRule);
                throw;
            }

            sw.Stop();
            this.Context.Logger.AppendLine(string.Format("ValidateBusinessRules Done: {0} ms", sw.ElapsedMilliseconds));
        }

        private void StartExecuteCrudOperation()
        {
            try
            {
                ExecuteCrudOperation();
            }
            catch (Exception ex)
            {
               // DAL.LogException(ex, LogCategory.CrudOperation);
                throw ex;
              //  throw new InvalidDataException("Invalid data. Please check your values.");
            }
        }

        private void StartExecuteModelEventLogging()
        {
            try
            {
                //if (WebApiConfig.IsModelEventLogsEnabled)
                //{
                //    ExecuteModelEventLogging();
                //}
            }
            catch (Exception ex)
            {
                //DAL.LogException(ex, LogCategory.ModelEvent);
            }
        }

        private void ExecuteCrudRules()
        {
            this.Context.Logger.AppendLine("ExecuteCrudRules Started");
            var sw = Stopwatch.StartNew();

            var crudRules = this.GetCrudRules().OrderByDescending(x => x.Priority);

            foreach (var rule in crudRules)
            {
                this.Context.Logger.AppendLine(string.Format("Executing {0}", rule));
                var innerSw = Stopwatch.StartNew();

                try
                {
                    rule.Execute();
                }
                catch (Exception ex)
                {
                    this.Context.Logger.AppendLine(string.Format("Error Executing {0}: {1}", rule, ex.Message));
                   // DAL.LogException(ex, LogCategory.CrudRule);
                }

                innerSw.Stop();
                this.Context.Logger.AppendLine(string.Format("{0} Done: {1} ms", rule, innerSw.ElapsedMilliseconds));
            }

            sw.Stop();
            this.Context.Logger.AppendLine(string.Format("ExecuteCrudRules Done: {0} ms", sw.ElapsedMilliseconds));
        }

        //private void ExecuteCrudHooks()
        //{
        //    try
        //    {
        //        this.Context.GetCurrentApplicationId();
        //    }
        //    catch (Exception)
        //    {
        //        this.Context.Logger.AppendLine("CrudHooks Skipped, Unknown Application");
        //        return;
        //    }

        //    this.Context.Logger.AppendLine("ExecuteCrudHooks Started");
        //    var sw = Stopwatch.StartNew();

        //    var applicationCrudHooks = ApplicationCrudHooksCachingService.GetCurrent()
        //            .GetCrudHookValuesForApplication(this.Context.GetCurrentApplicationId());

        //    var crudHooks = this.GetCrudHooks()
        //        .Where(x => applicationCrudHooks.Contains(x.CrudHookValue))
        //        .OrderByDescending(x => x.Priority);

        //    foreach (var hook in crudHooks)
        //    {
        //        this.Context.Logger.AppendLine(string.Format("Executing {0}", hook));
        //        var innerSw = Stopwatch.StartNew();

        //        try
        //        {
        //            hook.Execute();
        //        }
        //        catch (Exception ex)
        //        {
        //            this.Context.Logger.AppendLine(string.Format("Error Executing {0}: {1}", hook, ex.Message));
        //            DAL.LogException(ex, LogCategory.CrudHook);
        //        }

        //        innerSw.Stop();
        //        this.Context.Logger.AppendLine(string.Format("{0} Done: {1} ms", hook, innerSw.ElapsedMilliseconds));
        //    }

        //    sw.Stop();
        //    this.Context.Logger.AppendLine(string.Format("ExecuteCrudHooks Done: {0} ms", sw.ElapsedMilliseconds));
        //}







        protected abstract void ValidateApplication();

        protected abstract void ValidateUser();

        protected virtual void ValidateCollection()
        {
            if (!this.Context.GetCollection<T>().Any(x => x.Id == this.Model.Id))
            {
                throw new ModelNotFoundException();
            }
        }

        protected virtual void ExecuteCrudOperation() { }

        protected virtual void ExecuteModelEventLogging() { }
    }
}
