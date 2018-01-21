using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules
{
    /// <summary>
    /// A DataRule that will only be applied when the given model is going to be deleted.
    /// Don't assume that the model's navigation properties will be available.
    /// </summary>
    /// <typeparam name="T">Type of Model being validated.</typeparam>
    abstract class DeleteRule<T> : DataRule<T> where T : Model
    {
        protected DeleteRule(T model)
            : base(model)
        {
        }
    }
}
