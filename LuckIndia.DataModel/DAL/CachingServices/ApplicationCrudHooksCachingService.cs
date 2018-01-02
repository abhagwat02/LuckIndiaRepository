using System.Collections.Generic;
using Alphaeon.Services.EnterpriseAPI.DAL.CrudHooks;
using Alphaeon.Services.EnterpriseAPI.Interfaces;

namespace Alphaeon.Services.EnterpriseAPI.DAL.CachingServices
{
    sealed class ApplicationCrudHooksCachingService : IRefreshable
    {
        private readonly object _lockObject = new object();

        private readonly Dictionary<int, HashSet<CrudHookValue>> _cache = new Dictionary<int, HashSet<CrudHookValue>>();

        private ApplicationCrudHooksCachingService()
        {
            this.Refresh();
        }

        public IEnumerable<CrudHookValue> GetCrudHookValuesForApplication(int applicationId)
        {
            if (!_cache.ContainsKey(applicationId))
            {
                return new List<CrudHookValue>();
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
                    foreach (var applicationCrudHook in context.ApplicationCrudHooks)
                    {
                        var applicationId = applicationCrudHook.ApplicationId;
                        var crudHookId = applicationCrudHook.CrudHookId;

                        if (!_cache.ContainsKey(applicationId))
                        {
                            _cache.Add(applicationId, new HashSet<CrudHookValue>());
                        }

                        _cache[applicationId].Add((CrudHookValue)crudHookId);
                    }
                }
            }
        }

        private readonly static ApplicationCrudHooksCachingService CachingService = new ApplicationCrudHooksCachingService();

        public static ApplicationCrudHooksCachingService GetCurrent()
        {
            return CachingService;
        }
    }
}
