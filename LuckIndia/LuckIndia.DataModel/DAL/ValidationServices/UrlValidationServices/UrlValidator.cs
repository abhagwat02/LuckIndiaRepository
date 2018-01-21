using System;
using Alphaeon.Services.EnterpriseAPI.Interfaces;

namespace Alphaeon.Services.EnterpriseAPI.DAL.ValidationServices.UrlValidationServices
{
    abstract class UrlValidator : IValidatable
    {
        protected string Data { get; private set; }

        protected string PropertyName { get; private set; }

        protected UrlValidator(String validateMe, string propertyName)
        {
            this.Data = validateMe;
            this.PropertyName = propertyName;
        }

        public abstract void Validate();
    }
}
