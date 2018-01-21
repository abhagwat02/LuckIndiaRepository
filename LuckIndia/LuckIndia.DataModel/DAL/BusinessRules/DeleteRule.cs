using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.BusinessRules
{
    /// <summary>
    /// A DataRule that will only be applied when the given model is going to be deleted.
    /// The model will be fully navigateable if needed.
    /// </summary>
    /// <typeparam name="T">Type of Model being validated.</typeparam>
    abstract class DeleteRule<T> : BusinessRule<T> where T : Model
    {
        protected DeleteRule(T model, CMDDatabaseContext context)
            : base(model, context)
        {
        }
    }
}
