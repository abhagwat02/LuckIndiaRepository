using System.Linq;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.CollectionFilters
{
    abstract class RoleTypeCollectionFilter<T> : ICollectionFilter<T> where T : Model
    {
        public abstract int RoleTypeId { get; }

        public abstract IQueryable<T> ApplyFilter(CMDDatabaseContext context);
    }
}
