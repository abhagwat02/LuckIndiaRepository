using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.ApplicationCollectionFilters;
using Alphaeon.Services.EnterpriseAPI.DAL.Enums;
using Alphaeon.Services.EnterpriseAPI.Interfaces;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.CachingServices
{
    sealed class CollectionFilterCachingService : IRefreshable
    {
        private readonly ConcurrentDictionary<Type, IEnumerable<Type>> _cache = new ConcurrentDictionary<Type, IEnumerable<Type>>();

        private CollectionFilterCachingService() 
        {
            this.Refresh();
        }


        public IEnumerable<CollectionFilter<T>> GetCollectionFilters<T>(IEnumerable<CollectionFilterValue> collectionFilterValues, IQueryable<T> collection) where T : Model
        {
            if (_cache.ContainsKey(typeof(T)))
            {
                return _cache[typeof(T)].Select(x => (CollectionFilter<T>)Activator.CreateInstance(x, collection)).Where(x => collectionFilterValues.Contains(x.CollectionFilterValue));
            }

            return new List<CollectionFilter<T>>();
        }


        public void Refresh()
        {
            _cache.Clear();
            var cachingServiceReflector = new CachingServiceReflector(typeof(ApplicationCollectionFilters.CollectionFilter<>));
            foreach (var item in cachingServiceReflector.GetCache())
            {
                _cache.TryAdd(item.Key, item.Value);
            }
        }

        private readonly static CollectionFilterCachingService CachingService = new CollectionFilterCachingService();
        public static CollectionFilterCachingService GetCurrent()
        {
            return CachingService;
        }
    }
}