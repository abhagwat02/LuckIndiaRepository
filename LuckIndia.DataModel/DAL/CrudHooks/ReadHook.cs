using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.CrudHooks
{
    /// <summary>
    /// A CrudHook that will only be executed after the given model was read.
    /// The model's navigation properties will be available.
    /// </summary>
    /// <typeparam name="T">Type of Model to trigger the CrudHook.</typeparam>
    abstract class ReadHook<T> : CrudHook<T> where T : Model
    {
        protected ReadHook(T model, CMDDatabaseContext context)
            : base(model, context)
        {
        }
    }
}
