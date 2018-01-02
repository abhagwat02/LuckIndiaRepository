using LuckIndia.DataModel.DAL.CrudRules;
using LuckIndia.DataModel.Interfaces;
using LuckIndia.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LuckIndia.DataModel.DAL.CachingServices

{
    sealed class CrudRulesCachingService : IRefreshable
    {
        private readonly object _lockObject = new object();

        private Dictionary<Type, IEnumerable<Type>> _createCache = new Dictionary<Type, IEnumerable<Type>>();
        private Dictionary<Type, IEnumerable<Type>> _readCache = new Dictionary<Type, IEnumerable<Type>>();
        private Dictionary<Type, IEnumerable<Type>> _updateCache = new Dictionary<Type, IEnumerable<Type>>();
        private Dictionary<Type, IEnumerable<Type>> _deleteCache = new Dictionary<Type, IEnumerable<Type>>();

        public CrudRulesCachingService()
        {
            this.Refresh();
        }

        public void Refresh()
        {
            lock (_lockObject)
            {
                _createCache = (new CachingServiceReflector(typeof(CreateRule<>))).GetCache();
                _readCache = (new CachingServiceReflector(typeof(ReadRule<>))).GetCache();
                _updateCache = (new CachingServiceReflector(typeof(UpdateRule<>))).GetCache();
                _deleteCache = (new CachingServiceReflector(typeof(DeleteRule<>))).GetCache();
            }
        }

        public IEnumerable<CreateRule<T>> GetCreateRules<T>(T model, LuckIndiaDBContext context) where T : Model
        {
            var type = typeof(T);

            if (_createCache.ContainsKey(type))
            {
                return _createCache[type].Select(x => (CreateRule<T>)Activator.CreateInstance(x, model, context));
            }

            return new List<CreateRule<T>>();
        }

        public IEnumerable<ReadRule<T>> GetReadRules<T>(T model, LuckIndiaDBContext context) where T : Model
        {
            var type = typeof(T);

            if (_readCache.ContainsKey(type))
            {
                return _readCache[type].Select(x => (ReadRule<T>)Activator.CreateInstance(x, model, context));
            }

            return new List<ReadRule<T>>();
        }

        public IEnumerable<UpdateRule<T>> GetUpdateRules<T>(T model, IDictionary<string, object> oldValues, LuckIndiaDBContext context) where T : Model
        {
            var type = typeof(T);

            if (_updateCache.ContainsKey(type))
            {
                return _updateCache[type].Select(x => (UpdateRule<T>)Activator.CreateInstance(x, model, oldValues, context));
            }

            return new List<UpdateRule<T>>();
        }

        public IEnumerable<DeleteRule<T>> GetDeleteRules<T>(T model, LuckIndiaDBContext context) where T : Model
        {
            var type = typeof(T);

            if (_deleteCache.ContainsKey(type))
            {
                return _deleteCache[type].Select(x => (DeleteRule<T>)Activator.CreateInstance(x, model, context));
            }

            return new List<DeleteRule<T>>();
        }

        private readonly static CrudRulesCachingService CachingService = new CrudRulesCachingService();

        public static CrudRulesCachingService GetCurrent()
        {
            return CachingService;
        }
    }
}
