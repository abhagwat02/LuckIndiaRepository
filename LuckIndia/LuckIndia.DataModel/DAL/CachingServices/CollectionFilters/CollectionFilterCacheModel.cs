using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.CachingServices.CollectionFilters
{
    sealed class CollectionFilterCacheModel
    {
        public CollectionFilter CollectionFilter { get; set; }

        public int ModelClassId { get; set; }
    }
}