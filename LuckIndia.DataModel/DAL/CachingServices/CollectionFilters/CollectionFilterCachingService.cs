using System.Collections.Concurrent;
using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Interfaces;

namespace Alphaeon.Services.EnterpriseAPI.DAL.CachingServices.CollectionFilters
{
    sealed class CollectionFilterCachingService : IRefreshable
    {
        private readonly ConcurrentDictionary<int, CollectionFilterCacheModel> _cache = new ConcurrentDictionary<int,CollectionFilterCacheModel>();

        private CollectionFilterCachingService() { }
        
        public CollectionFilterCacheModel GetCollectionFilterCacheModel(int collectionFilterId)
        {
            if (_cache.ContainsKey(collectionFilterId))
            {
                return _cache[collectionFilterId];
            }

            this.RefreshCacheForCollectionFilterId(collectionFilterId);

            return _cache[collectionFilterId];
        }

        private void RefreshCacheForCollectionFilterId(int collectionFilterId)
        {
            using (var context = CMDDatabaseContext.GetContext())
            {
                var collectionFilter = context.CollectionFilters.FirstOrDefault(x => x.Id == collectionFilterId);
                if (collectionFilter == null)
                {
                    throw new InvalidDataException();
                }

                var collectionFilterCache = new CollectionFilterCacheModel
                {
                    CollectionFilter = collectionFilter,
                    ModelClassId = collectionFilter.ModelClassId
                };

                _cache.TryAdd(collectionFilterId, collectionFilterCache);
            }
        }

        public void RemoveFromCache(int collectionFilterId)
        {
            CollectionFilterCacheModel cacheModel;
            _cache.TryRemove(collectionFilterId, out cacheModel);
        }

        public void Refresh()
        {
            // Empty the cache?
        }

        private readonly static CollectionFilterCachingService CachingService = new CollectionFilterCachingService();
        public static CollectionFilterCachingService GetCurrent()
        {
            return CachingService;
        }
    }
}