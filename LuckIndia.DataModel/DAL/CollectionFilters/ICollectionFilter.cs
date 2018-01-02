using System.Linq;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.CollectionFilters
{
    interface ICollectionFilter<T> where T : Model
    {
        IQueryable<T> ApplyFilter(CMDDatabaseContext context);
    }
}
