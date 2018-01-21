using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.CollectionFilters
{
    sealed class TitleMustBeUniquePerModelClassCreateRule : CreateRule<CollectionFilter>
    {
        public  TitleMustBeUniquePerModelClassCreateRule(CollectionFilter model)
            : base(model)
        {
        }

        public override void Validate()
        {
            using (var context = CMDDatabaseContext.GetContext())
            {
                if (context.CollectionFilters.Any(c => c.Title == this.Model.Title && c.ModelClassId == this.Model.ModelClassId))
                {
                    throw new InvalidDataException("CollectionFilter Title must be unique per ModelClass.");
                }
            }
        }
    }
}