using System.Linq;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.CollectionFilters
{
    abstract class RoleCollectionFilter<T> : ICollectionFilter<T> where T : Model
    {
        public abstract int RoleId { get; }

        public abstract IQueryable<T> ApplyFilter(CMDDatabaseContext context);
    }
}
