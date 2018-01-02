using System.Collections.Generic;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.CrudHooks
{
    /// <summary>
    /// A CrudHook that will only be executed after the given model was updated.
    /// The model's navigation properties will be available.
    /// </summary>
    /// <typeparam name="T">Type of Model to trigger the CrudHook.</typeparam>
    abstract class UpdateHook<T> : CrudHook<T> where T : Model
    {
        protected UpdateHook(T model, IDictionary<string, object> oldValues, CMDDatabaseContext context)
            : base(model, context)
        {
            this.OldValues = oldValues;
        }

        protected IDictionary<string, object> OldValues { get; private set; }
    }
}
