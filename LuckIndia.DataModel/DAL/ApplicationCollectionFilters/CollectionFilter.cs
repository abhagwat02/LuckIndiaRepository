using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.Enums;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.ApplicationCollectionFilters
{
    abstract class CollectionFilter<T> where T : Model
    {
        protected IQueryable<T> Collection { get; private set; }

        protected CollectionFilter(IQueryable<T> collection)
        {
            this.Collection = collection;
        }

        public abstract CollectionFilterValue CollectionFilterValue { get; }

        public abstract IQueryable<T> ApplyFilter();
    }
}