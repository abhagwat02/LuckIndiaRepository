using Alphaeon.Services.EnterpriseAPI.ApplicationServices.HashingServices;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.Users
{
    sealed class SetModelPasswordHashIfInPlainTextCreateRule : CreateRule<User>
    {
        public SetModelPasswordHashIfInPlainTextCreateRule(User model)
            : base(model)
        {
        }

        public override void Validate()
        {
            if (this.Model.PasswordHash == null)
            {
                return;
            }

            var plainTextPassword = this.Model.PasswordHash;

            PasswordRules.PasswordRules.ValidatePassword(plainTextPassword);

            var hashedPassword = HashingService.Hash(plainTextPassword, this.Model.PasswordSalt);

            //reset the model to the hashed version
            this.Model.PasswordHash = hashedPassword;
        }
    }
}
