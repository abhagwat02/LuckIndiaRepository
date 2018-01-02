using System.Collections.Generic;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.Users
{
    sealed class EmailOrUsernameRequiredUpdateRule : UpdateRule<User>
    {
        public EmailOrUsernameRequiredUpdateRule(User model, IDictionary<string, object> delta)
            : base(model, delta)
        {
        }

        public override void Validate()
        {
            var email = this.Model.Email;
            var userName = this.Model.UserName;

            var emailKey = DAL.PropertyName<User>(x => x.Email);
            if (this.Delta.ContainsKey(emailKey))
            {
                email = this.Delta[emailKey] as string;
            }

            var userNameKey = DAL.PropertyName<User>(x => x.UserName);
            if (this.Delta.ContainsKey(userNameKey))
            {
                userName = this.Delta[userNameKey] as string;
            }

            if (string.IsNullOrWhiteSpace(userName) && string.IsNullOrWhiteSpace(email))
            {
                throw new InvalidDataException("UserName or Email required.");
            }
        }
    }
}
