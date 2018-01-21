using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.DAL.ValidationServices.ExpirableValidationServices;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.AccessTokens
{
    sealed class ValidUserTokenCreateRule : CreateRule<AccessToken>
    {
        public ValidUserTokenCreateRule(AccessToken model)
            : base(model)
        {
        }

        public override void Validate()
        {
            using (var context = CMDDatabaseContext.GetContext())
            {
                var userToken = context.Users.FirstOrDefault(x => x.Id == this.Model.UserId);

                if (userToken == null )
                {
                    throw new InvalidDataException("Invalid UserToken.");
                }
            }
        }
    }
}
