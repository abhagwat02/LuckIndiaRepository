using LuckIndia.DataModel.DAL.CachingServices;
using LuckIndia.DataModel.DAL.CrudRules;
using LuckIndia.DataModel.Interfaces;
using LuckIndia.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Alphaeon.Services.EnterpriseAPI.DAL.CachingServices
{
    sealed class DataRulesCachingService : IRefreshable
    {
        private readonly object _lockObject = new object();

        private Dictionary<Type, IEnumerable<Type>> _createCache = new Dictionary<Type, IEnumerable<Type>>();
        private Dictionary<Type, IEnumerable<Type>> _updateCache = new Dictionary<Type, IEnumerable<Type>>();
        private Dictionary<Type, IEnumerable<Type>> _deleteCache = new Dictionary<Type, IEnumerable<Type>>();

        public DataRulesCachingService()
        {
            this.Refresh();
        }

        public void Refresh()
        {
            lock (_lockObject)
            {
                _createCache = (new CachingServiceReflector(typeof(CreateRule<>))).GetCache();
                _updateCache = (new CachingServiceReflector(typeof(UpdateRule<>))).GetCache();
                _deleteCache = (new CachingServiceReflector(typeof(DeleteRule<>))).GetCache();
            }
        }

        public IEnumerable<CreateRule<T>> GetCreateRules<T>(T model) where T : Model
        {
            var type = typeof(T);

            if (_createCache.ContainsKey(type))
            {
                return _createCache[type].Select(x => (CreateRule<T>)Activator.CreateInstance(x, model));
            }

            return new List<CreateRule<T>>();
        }

        public IEnumerable<UpdateRule<T>> GetUpdateRules<T>(T model, IDictionary<string, object> delta) where T : Model
        {
            var type = typeof(T);

            if (_updateCache.ContainsKey(type))
            {
                return _updateCache[type].Select(x => (UpdateRule<T>)Activator.CreateInstance(x, model, delta));
            }

            return new List<UpdateRule<T>>();
        }

        public IEnumerable<DeleteRule<T>> GetDeleteRules<T>(T model) where T : Model
        {
            var type = typeof(T);

            if (_deleteCache.ContainsKey(type))
            {
                return _deleteCache[type].Select(x => (DeleteRule<T>)Activator.CreateInstance(x, model));
            }

            return new List<DeleteRule<T>>();
        }

        private readonly static DataRulesCachingService CachingService = new DataRulesCachingService();

        public static DataRulesCachingService GetCurrent()
        {
            return CachingService;
        }
    }
}
