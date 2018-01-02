using System;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;

namespace Alphaeon.Services.EnterpriseAPI.DAL.ValidationServices.StringValidationServices
{
    sealed class NotNullValidator : StringValidator
    {
        public NotNullValidator(String validateMe, string propertyName)
            : base(validateMe, propertyName) { }

        public override void Validate()
        {
            if (null == Data)
            {
                throw new NullOrEmptyPropertyException(this.PropertyName);
            }
        }
    }
}
