using System.Collections.Generic;
using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.CollectionFilters
{
    sealed class TitleMustBeUniquePerModelClassUpdateRule : UpdateRule<CollectionFilter>
    {
        public TitleMustBeUniquePerModelClassUpdateRule(CollectionFilter model, IDictionary<string, object> delta)
            : base(model, delta)
        {
        }

        public override void Validate()
        {
            var titleKey = DAL.PropertyName<CollectionFilter>(x => x.Title);
            if (!this.Delta.ContainsKey(titleKey))
            {
                return;
            }

            var newTitle = this.Delta[titleKey].ToString();

            using (var context = CMDDatabaseContext.GetContext())
            {
                if (context.CollectionFilters.Any(c => c.Title == newTitle && c.ModelClassId == this.Model.ModelClassId))
                {
                    throw new InvalidDataException("CollectionFilter Title must be unique per ModelClass.");
                }
            }
        }
    }
}
