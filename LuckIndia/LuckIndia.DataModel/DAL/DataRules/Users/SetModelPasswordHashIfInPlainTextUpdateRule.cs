using System.Collections.Generic;
using Alphaeon.Services.EnterpriseAPI.ApplicationServices.HashingServices;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.Users
{
    sealed class SetModelPasswordHashIfInPlainTextUpdateRule : UpdateRule<User>
    {
        public SetModelPasswordHashIfInPlainTextUpdateRule(User model, IDictionary<string, object> delta)
            : base(model, delta)
        {
        }

        public override void Validate()
        {
            var passwordKey = DAL.PropertyName<User>(x => x.PasswordHash);
            if (!this.Delta.ContainsKey(passwordKey))
            {
                return;
            }

            var plainTextPassword = this.Delta[passwordKey].ToString();

            PasswordRules.PasswordRules.ValidatePassword(plainTextPassword);

            var hashedPassword = HashingService.Hash(plainTextPassword, this.Model.PasswordSalt);

            //reset the delta to the hashed version
            this.Delta[passwordKey] = hashedPassword;
        }
    }
}
