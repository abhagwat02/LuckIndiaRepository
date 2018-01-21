using System;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;

namespace Alphaeon.Services.EnterpriseAPI.DAL.ValidationServices.StringValidationServices
{
    sealed class NotNullOrWhiteSpaceValidator : StringValidator
    {
        public NotNullOrWhiteSpaceValidator(string validateMe, string propertyName)
            : base(validateMe, propertyName) { }

        public override void Validate()
        {
            if (String.IsNullOrWhiteSpace(Data))
            {
                throw new NullOrEmptyPropertyException(this.PropertyName);
            }
        }
    }
}
