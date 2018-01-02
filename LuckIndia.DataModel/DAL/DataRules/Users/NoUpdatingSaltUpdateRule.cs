using System.Collections.Generic;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.Users
{
    sealed class NoUpdatingSaltUpdateRule : UpdateRule<User>
    {
        public NoUpdatingSaltUpdateRule(User model, IDictionary<string, object> delta)
            : base(model, delta)
        {
        }

        public override void Validate()
        {
            var saltKey = DAL.PropertyName<User>(x => x.PasswordSalt);
            if (this.Delta.ContainsKey(saltKey))
            {
                throw new InvalidDataException("PasswordSalt can't be updated.");
            }
        }
    }
}
