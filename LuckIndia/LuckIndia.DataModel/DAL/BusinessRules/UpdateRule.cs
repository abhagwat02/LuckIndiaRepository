using System.Collections.Generic;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.BusinessRules
{
    /// <summary>
    /// A DataRule that will only be applied when the given model is going to be updated.
    /// The model will be fully navigateable if needed.
    /// The delta will contain the properties that are going to be applied, but not yet applied.
    /// </summary>
    /// <typeparam name="T">Type of Model being validated.</typeparam>
    abstract class UpdateRule<T> : BusinessRule<T> where T : Model
    {
        protected IDictionary<string, object> Delta { get; private set; }

        protected UpdateRule(T model, IDictionary<string, object> delta, CMDDatabaseContext context)
            : base(model, context)
        {
            this.Delta = delta;
        }
    }
}
