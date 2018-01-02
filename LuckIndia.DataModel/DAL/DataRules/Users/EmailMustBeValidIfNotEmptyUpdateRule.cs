using System.Collections.Generic;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.DAL.ValidationServices.EmailValidationServices;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.Users
{
    sealed class EmailMustBeValidIfNotEmptyUpdateRule : UpdateRule<User>
    {
        public EmailMustBeValidIfNotEmptyUpdateRule(User model, IDictionary<string, object> delta)
            : base(model, delta)
        {
        }

        public override void Validate()
        {
            var emailKey = DAL.PropertyName<User>(x => x.Email);
            if (!this.Delta.ContainsKey(emailKey))
            {
                return;
            }

            var email = this.Delta[emailKey] as string;

            if (string.IsNullOrEmpty(email))
            {
                return;
            }

            if (!EmailValidator.IsValid(email))
            {
                throw new InvalidDataException("Invalid email address.");
            }
        }
    }
}
