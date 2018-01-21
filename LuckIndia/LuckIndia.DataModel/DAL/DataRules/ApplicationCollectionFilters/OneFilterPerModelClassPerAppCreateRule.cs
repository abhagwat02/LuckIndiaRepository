using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.ApplicationCollectionFilters
{
    sealed class OneFilterPerModelClassPerAppCreateRule : CreateRule<ApplicationCollectionFilter>
    {
        public OneFilterPerModelClassPerAppCreateRule(ApplicationCollectionFilter model) 
            : base(model)
        {
        }

        public override void Validate()
        {
            using (var context = CMDDatabaseContext.GetContext())
            {
                //todo: Question - is the following navigation possible?
                var collectionFilter = context.CollectionFilters.FirstOrDefault(c => c.Id == this.Model.CollectionFilterId);
                if (collectionFilter == null)
                {
                    throw new InvalidDataException("Invalid CollectionFilterId.");
                }
                if (context.ApplicationCollectionFilters.Any(x => x.CollectionFilter.ModelClassId == collectionFilter.ModelClassId && x.ApplicationId == this.Model.ApplicationId))
                {
                    throw new InvalidDataException("Application already has a CollectionFilter applied to this model class.");
                }
            }
        }
    }
}