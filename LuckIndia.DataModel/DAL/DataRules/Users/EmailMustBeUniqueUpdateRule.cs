using System.Collections.Generic;
using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.Users
{
    sealed class EmailMustBeUniqueUpdateRule : UpdateRule<User>
    {
        public EmailMustBeUniqueUpdateRule(User model, IDictionary<string, object> delta)
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

            if (string.IsNullOrWhiteSpace(email))
            {
                return;
            }

            using (var context = CMDDatabaseContext.GetContext())
            {
                if (context.Users.Any(x => x.Email == email && x.Id != this.Model.Id))
                {
                    throw new InvalidDataException("Email address already exists in the system.");
                }
            }
        }
    }
}
