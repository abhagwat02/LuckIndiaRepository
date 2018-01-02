using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.DAL.ValidationServices.ExpirableValidationServices;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.AccessTokens
{
    sealed class ValidApplicationTokenCreateRule : CreateRule<AccessToken>
    {
        public ValidApplicationTokenCreateRule(AccessToken model)
            : base(model)
        {
        }

        public override void Validate()
        {
            using (var context = CMDDatabaseContext.GetContext())
            {
                var applicationToken = context.Applications.FirstOrDefault(x => x.Id == this.Model.ApplicationId);

                if (applicationToken == null )
                {
                    throw new InvalidDataException("Invalid ApplicationToken.");
                }
            }
        }
    }
}
