using System.Collections.Generic;
using System.Linq;
using Alphaeon.Services.EnterpriseAPI.Enums;
using Alphaeon.Services.EnterpriseAPI.Interfaces;

namespace Alphaeon.Services.EnterpriseAPI.DAL.CachingServices
{
    public class ApplicationPermissionCachingService : IRefreshable
    {
        private readonly object _lockObject = new object();

        private readonly Dictionary<int, HashSet<int>> _createCache = new Dictionary<int, HashSet<int>>();
        private readonly Dictionary<int, HashSet<int>> _readCache = new Dictionary<int, HashSet<int>>();
        private readonly Dictionary<int, HashSet<int>> _updateCache = new Dictionary<int, HashSet<int>>();
        private readonly Dictionary<int, HashSet<int>> _deleteCache = new Dictionary<int, HashSet<int>>();

        private const int BaseApplicationId = (int)ApplicationValue.BasePermissions;

        private ApplicationPermissionCachingService()
        {
            this.Refresh();
        }

        public bool CanCreate(int applicationId, int modelClassId)
        {
            if (!_createCache.ContainsKey(modelClassId))
            {
                return false;
            }

            return _createCache[modelClassId].Any(x => x == BaseApplicationId || x == applicationId);
        }

        public bool CanRead(int applicationId, int modelClassId)
        {
            if (!_readCache.ContainsKey(modelClassId))
            {
                return false;
            }

            return _readCache[modelClassId].Any(x => x == BaseApplicationId || x == applicationId);
        }

        public bool CanUpdate(int applicationId, int modelClassId)
        {
            if (!_updateCache.ContainsKey(modelClassId))
            {
                return false;
            }

            return _updateCache[modelClassId].Any(x => x == BaseApplicationId || x == applicationId);
        }

        public bool CanDelete(int applicationId, int modelClassId)
        {
            if (!_deleteCache.ContainsKey(modelClassId))
            {
                return false;
            }

            return _deleteCache[modelClassId].Any(x => x == BaseApplicationId || x == applicationId);
        }

        public void Refresh()
        {
            lock (_lockObject)
            {
                _createCache.Clear();
                _readCache.Clear();
                _updateCache.Clear();
                _deleteCache.Clear();

                using (var context = CMDDatabaseContext.GetContext())
                {
                    foreach (var rolePermission in context.ApplicationPermissions)
                    {
                        var modelClassId = rolePermission.ModelClassId;
                        var roleId = rolePermission.ApplicationId;

                        if (rolePermission.CanCreate)
                        {
                            if (!_createCache.ContainsKey(modelClassId))
                            {
                                _createCache.Add(modelClassId, new HashSet<int>());
                            }

                            _createCache[modelClassId].Add(roleId);
                        }

                        if (rolePermission.CanRead)
                        {
                            if (!_readCache.ContainsKey(modelClassId))
                            {
                                _readCache.Add(modelClassId, new HashSet<int>());
                            }

                            _readCache[modelClassId].Add(roleId);
                        }

                        if (rolePermission.CanUpdate)
                        {
                            if (!_updateCache.ContainsKey(modelClassId))
                            {
                                _updateCache.Add(modelClassId, new HashSet<int>());
                            }

                            _updateCache[modelClassId].Add(roleId);
                        }

                        if (rolePermission.CanDelete)
                        {
                            if (!_deleteCache.ContainsKey(modelClassId))
                            {
                                _deleteCache.Add(modelClassId, new HashSet<int>());
                            }

                            _deleteCache[modelClassId].Add(roleId);
                        }
                    }
                }
            }
        }

        private readonly static ApplicationPermissionCachingService CachingService = new ApplicationPermissionCachingService();

        public static ApplicationPermissionCachingService GetCurrent()
        {
            return CachingService;
        }
    }
}
