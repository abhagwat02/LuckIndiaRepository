using LuckIndia.Models;
using System.Collections.Generic;

namespace LuckIndia.DataModel.DAL.CrudRules

{
    /// <summary>
    /// A CrudRule that will only be executed after the given model was updated.
    /// The model's navigation properties will be available.
    /// </summary>
    /// <typeparam name="T">Type of Model to trigger the CrudRule.</typeparam>
    abstract class UpdateRule<T> : CrudRule<T> where T : Model
    {
        protected UpdateRule(T model, IDictionary<string, object> oldValues, LuckIndiaDBContext context)
            : base(model, context)
        {
            this.OldValues = oldValues;
        }

        protected IDictionary<string, object> OldValues { get; private set; }
    }
}
