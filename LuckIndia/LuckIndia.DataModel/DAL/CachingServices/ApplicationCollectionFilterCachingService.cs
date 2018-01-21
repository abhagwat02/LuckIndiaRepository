using System.Collections.Generic;
using Alphaeon.Services.EnterpriseAPI.DAL.Enums;
using Alphaeon.Services.EnterpriseAPI.Interfaces;

namespace Alphaeon.Services.EnterpriseAPI.DAL.CachingServices
{
    sealed class ApplicationCollectionFilterCachingService : IRefreshable
    {
        private readonly object _lockObject = new object();

        private readonly Dictionary<int, HashSet<CollectionFilterValue>> _cache = new Dictionary<int, HashSet<CollectionFilterValue>>();

        private ApplicationCollectionFilterCachingService()
        {
            this.Refresh();
        }

        public IEnumerable<CollectionFilterValue> GetCollectionFilterValuesForApplication(int applicationId)
        {
            if (!_cache.ContainsKey(applicationId))
            {
                return new List<CollectionFilterValue>();
            }

            return _cache[applicationId];
        }

        public void Refresh()
        {
            lock (_lockObject)
            {
                _cache.Clear();

                using (var context = CMDDatabaseContext.GetContext())
                {
                    foreach(var applicationCollectionFilter in context.ApplicationCollectionFilters)
                    {
                        var applicationId = applicationCollectionFilter.ApplicationId;
                        var collectionFilterId = applicationCollectionFilter.CollectionFilterId;

                        if (!_cache.ContainsKey(applicationId))
                        {
                            _cache.Add(applicationId, new HashSet<CollectionFilterValue>());
                        }

                        _cache[applicationId].Add((CollectionFilterValue)collectionFilterId);
                    }
                }
            }
        }


        private readonly static ApplicationCollectionFilterCachingService CachingService = new ApplicationCollectionFilterCachingService();

        public static ApplicationCollectionFilterCachingService GetCurrent()
        {
            return CachingService;
        }
    }
}