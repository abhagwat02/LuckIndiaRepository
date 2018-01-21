using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.Users
{
    sealed class EmailAddressAlreadyInUseCreateRule : CreateRule<User>
    {
        public EmailAddressAlreadyInUseCreateRule(User model)
            : base(model)
        {
        }

        public override void Validate()
        {
            if (string.IsNullOrWhiteSpace(this.Model.Email))
            {
                return;
            }

            using (var context = CMDDatabaseContext.GetContext())
            {
                if (context.Users.Any(u => u.Email == this.Model.Email))
                {
                    throw new InvalidDataException("Email address is already in use!");
                }
            }
        }
    }
}