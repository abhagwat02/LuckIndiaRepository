using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.ApplicationCollectionFilters
{
    sealed class ApplicationCannotAlreadyHaveThisCollectionFilterCreateRule : CreateRule<ApplicationCollectionFilter>
    {
        public ApplicationCannotAlreadyHaveThisCollectionFilterCreateRule(ApplicationCollectionFilter model)
            : base(model)
        {
        }

        public override void Validate()
        {
            using (var context = CMDDatabaseContext.GetContext())
            {
                if (context.ApplicationCollectionFilters.Any(x => x.ApplicationId == this.Model.ApplicationId && x.CollectionFilterId == this.Model.CollectionFilterId)) {
                    throw new InvalidDataException("Application already has this CollectionFilter applied.");
                }
            }
        }
    }
}