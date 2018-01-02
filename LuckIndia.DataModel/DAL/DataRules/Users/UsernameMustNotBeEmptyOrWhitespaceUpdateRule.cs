using System.Collections.Generic;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.Users
{
    sealed class UsernameMustNotBeEmptyOrWhitespaceUpdateRule : UpdateRule<User>
    {
        public UsernameMustNotBeEmptyOrWhitespaceUpdateRule(User model, IDictionary<string, object> delta)
            : base(model, delta)
        {
        }

        public override void Validate()
        {
            var userNameKey = DAL.PropertyName<User>(x => x.UserName);
            if (!this.Delta.ContainsKey(userNameKey))
            {
                return;
            }

            var userName = this.Delta[userNameKey] as string;
            if (null == userName)
            {
                return;
            }

            if (userName.Trim() == string.Empty)
            {
                throw new InvalidDataException("Username must not be empty.");
            }
        }
    }
}