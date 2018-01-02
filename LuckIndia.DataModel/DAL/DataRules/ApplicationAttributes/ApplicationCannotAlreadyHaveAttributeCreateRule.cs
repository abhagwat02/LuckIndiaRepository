using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.ApplicationAttributes
{
    sealed class ApplicationCannotAlreadyHaveAttributeCreateRule : CreateRule<ApplicationAttribute>
    {
        public ApplicationCannotAlreadyHaveAttributeCreateRule(ApplicationAttribute model)
            : base(model)
        {
        }

        public override void Validate()
        {
            using (var context = CMDDatabaseContext.GetContext())
            {
                if (context.ApplicationAttributes.Any(x => x.ApplicationId == this.Model.ApplicationId && x.AttributeId == this.Model.AttributeId))
                {
                    throw new InvalidDataException("Application already has this attribute.");
                }
            }
        }
    }
}