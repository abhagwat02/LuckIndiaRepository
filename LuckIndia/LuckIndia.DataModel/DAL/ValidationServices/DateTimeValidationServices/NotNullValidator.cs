using System;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;

namespace Alphaeon.Services.EnterpriseAPI.DAL.ValidationServices.DateTimeValidationServices
{
    sealed class NotNullValidator : DateTimeValidator
    {
        public NotNullValidator(DateTime date, string invalidMessage) : base(date, invalidMessage)
        {

        }

        public override void Validate()
        {
            if(_data == null)
            {
                throw new InvalidDataException(_invalidMessage);
            }
        }
    }
}