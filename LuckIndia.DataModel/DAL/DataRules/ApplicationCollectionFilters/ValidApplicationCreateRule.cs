using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.ApplicationCollectionFilters
{
    sealed class ValidApplicationCreateRule : CreateRule<ApplicationCollectionFilter>
    {
        public ValidApplicationCreateRule(ApplicationCollectionFilter model) : base(model)
        {
        }

        public override void Validate()
        {
            using (var context = CMDDatabaseContext.GetContext())
            {
                if (!context.Applications.Any(a => a.Id == this.Model.ApplicationId))
                {
                    throw new InvalidDataException("Invalid ApplicationId.");
                }
            }
        }
    }
}