using Alphaeon.Services.EnterpriseAPI.Interfaces;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.BusinessRules
{
    /// <summary>
    /// Rules that will be validated during CRUD operations.
    /// Validate will typically verify properties against the current user and/or application.
    /// </summary>
    /// <typeparam name="T">Type of Model being validated.</typeparam>
    abstract class BusinessRule<T> : IValidatable where T : Model
    {
        protected BusinessRule(T model, CMDDatabaseContext context)
        {
            this.Model = model;
            this.Context = context;
        }

        protected T Model { get; private set; }

        protected CMDDatabaseContext Context { get; private set; }

        public abstract void Validate();
    }
}
