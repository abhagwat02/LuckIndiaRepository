using LuckIndia.DataModel;
using LuckIndia.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LuckIndia.DataModel.DAL.CachingServices
{
    sealed class ModelClassCachingService //: IRefreshable
    {
        private readonly object _lockObject = new object();
        private readonly Dictionary<string, int> _cache = new Dictionary<string, int>();
        private readonly HashSet<Type> _cacheTypes = new HashSet<Type>();

        private ModelClassCachingService()
        {
            this.Refresh();
        }

        public int GetModelClassId<T>() where T : Model
        {
            var name = typeof(T).Name;

            if (_cache.ContainsKey(name))
            {
                return _cache[name];
            }

            return -1;
        }

        public IEnumerable<Type> GetModelTypes()
        {
            return _cacheTypes;
        } 

        public void Refresh()
        {
            lock (_lockObject)
            {
                _cache.Clear();
                _cacheTypes.Clear();

                using (var context = LuckIndiaDBContext.GetContext())
                {
                    foreach (var modelClass in context.ModelClasses)
                    {
                        _cache.Add(modelClass.Title, modelClass.Id);
                    }
                }

                var types = Assembly.Load("LuckIndia.Models")//GetExecutingAssembly()
                    .GetTypes()
                    .Where(x => !x.IsAbstract && x.IsSubclassOf(typeof(Model))).ToList();

                foreach (var type in types)
                {
                    _cacheTypes.Add(type);
                }
            }
        }


        private readonly static ModelClassCachingService CachingService = new ModelClassCachingService();

        public static ModelClassCachingService GetCurrent()
        {
            return CachingService;
        }
    }
}
