using System.Collections.Generic;
using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.Users
{
    sealed class UsernameMustBeUniqueUpdateRule : UpdateRule<User>
    {
        public UsernameMustBeUniqueUpdateRule(User model, IDictionary<string, object> delta)
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
            if (string.IsNullOrWhiteSpace(userName))
            {
                return;
            }

            using (var context = CMDDatabaseContext.GetContext())
            {
                if (context.Users.Any(x => x.UserName == userName && x.Id != this.Model.Id))
                {
                    throw new InvalidDataException("Username already exists in the system.");
                }
            }
        }
    }
}
