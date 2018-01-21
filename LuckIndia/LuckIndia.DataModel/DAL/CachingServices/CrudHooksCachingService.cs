using System;
using System.Collections.Generic;
using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.CrudHooks;
using Alphaeon.Services.EnterpriseAPI.Interfaces;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.CachingServices
{
    sealed class CrudHooksCachingService : IRefreshable
    {
        private readonly object _lockObject = new object();

        private Dictionary<Type, IEnumerable<Type>> _createCache = new Dictionary<Type, IEnumerable<Type>>();
        private Dictionary<Type, IEnumerable<Type>> _readCache = new Dictionary<Type, IEnumerable<Type>>();
        private Dictionary<Type, IEnumerable<Type>> _updateCache = new Dictionary<Type, IEnumerable<Type>>();
        private Dictionary<Type, IEnumerable<Type>> _deleteCache = new Dictionary<Type, IEnumerable<Type>>();

        public CrudHooksCachingService()
        {
            this.Refresh();
        }

        public void Refresh()
        {
            lock (_lockObject)
            {
                _createCache = (new CachingServiceReflector(typeof(CreateHook<>))).GetCache();
                _readCache = (new CachingServiceReflector(typeof(ReadHook<>))).GetCache();
                _updateCache = (new CachingServiceReflector(typeof(UpdateHook<>))).GetCache();
                _deleteCache = (new CachingServiceReflector(typeof(DeleteHook<>))).GetCache();
            }
        }

        public IEnumerable<CreateHook<T>> GetCreateHooks<T>(T model, CMDDatabaseContext context) where T : Model
        {
            var type = typeof(T);

            if (_createCache.ContainsKey(type))
            {
                return _createCache[type].Select(x => (CreateHook<T>)Activator.CreateInstance(x, model, context));
            }

            return new List<CreateHook<T>>();
        }

        public IEnumerable<ReadHook<T>> GetReadHooks<T>(T model, CMDDatabaseContext context) where T : Model
        {
            var type = typeof(T);

            if (_readCache.ContainsKey(type))
            {
                return _readCache[type].Select(x => (ReadHook<T>)Activator.CreateInstance(x, model, context));
            }

            return new List<ReadHook<T>>();
        }

        public IEnumerable<UpdateHook<T>> GetUpdateHooks<T>(T model, IDictionary<string, object> oldValues, CMDDatabaseContext context) where T : Model
        {
            var type = typeof(T);

            if (_updateCache.ContainsKey(type))
            {
                return _updateCache[type].Select(x => (UpdateHook<T>)Activator.CreateInstance(x, model, oldValues, context));
            }

            return new List<UpdateHook<T>>();
        }

        public IEnumerable<DeleteHook<T>> GetDeleteHooks<T>(T model, CMDDatabaseContext context) where T : Model
        {
            var type = typeof(T);

            if (_deleteCache.ContainsKey(type))
            {
                return _deleteCache[type].Select(x => (DeleteHook<T>)Activator.CreateInstance(x, model, context));
            }

            return new List<DeleteHook<T>>();
        }

        private readonly static CrudHooksCachingService CachingService = new CrudHooksCachingService();

        public static CrudHooksCachingService GetCurrent()
        {
            return CachingService;
        }
    }
}
