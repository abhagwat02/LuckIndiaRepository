using LuckIndia.DataModel;
using LuckIndia.DataModel.Interfaces;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Alphaeon.Services.EnterpriseAPI.DAL.CachingServices
{
    public class RolePermissionCachingService : IRefreshable
    {
        private readonly object _lockObject = new object();

        private readonly Dictionary<int, HashSet<int>> _createCache = new Dictionary<int, HashSet<int>>();
        private readonly Dictionary<int, HashSet<int>> _readCache = new Dictionary<int, HashSet<int>>();
        private readonly Dictionary<int, HashSet<int>> _updateCache = new Dictionary<int, HashSet<int>>();
        private readonly Dictionary<int, HashSet<int>> _deleteCache = new Dictionary<int, HashSet<int>>();

        private readonly ConcurrentDictionary<int, HashSet<int>> _userRoleCache = new ConcurrentDictionary<int, HashSet<int>>();

        private RolePermissionCachingService()
        {
            this.Refresh();
        }

        public bool CanCreate(int modelClassId, LuckIndiaDBContext context)
        {
            //if (!_createCache.ContainsKey(modelClassId))
            //{
            //    return false;
            //}

            //// var userRoles = GetUserRoleIds(userId);
            //var userRoles = context.SecurityContext.User.Roles.Select(x => x.Id).ToList();
            //return _createCache[modelClassId].Intersect(userRoles).Any();
            return true;
        }

        public bool CanRead(int modelClassId, LuckIndiaDBContext context)
        {
            //if (!_readCache.ContainsKey(modelClassId))
            //{
            //    return false;
            //}

            //var userRoles = context.SecurityContext.User.Roles.Select(x => x.Id).ToList();
            //return _readCache[modelClassId].Intersect(userRoles).Any();
            return true;

        }

        public bool CanUpdate(int modelClassId, LuckIndiaDBContext context)
        {
            //if (!_updateCache.ContainsKey(modelClassId))
            //{
            //    return false;
            //}

            //// var userRoles = GetUserRoleIds(userId);
            //var userRoles = context.SecurityContext.User.Roles.Select(x => x.Id).ToList();
            //return _updateCache[modelClassId].Intersect(userRoles).Any();
            return true;

        }

        public bool CanDelete(int modelClassId, LuckIndiaDBContext context)
        {
            //if (!_deleteCache.ContainsKey(modelClassId))
            //{
            //    return false;
            //}

            //// var userRoles = GetUserRoleIds(userId);
            //var userRoles = context.SecurityContext.User.Roles.Select(x => x.Id).ToList();
            //return _deleteCache[modelClassId].Intersect(userRoles).Any();
            return true;

        }

        public void Refresh()
        {
            //lock (_lockObject)
            //{
            //    _createCache.Clear();
            //    _readCache.Clear();
            //    _updateCache.Clear();
            //    _deleteCache.Clear();

            //    using (var context = LuckIndiaDBContext.GetContext())
            //    {
            //        foreach (var rolePermission in context.RolePermissions)
            //        {
            //            var modelClassId = rolePermission.ModelClassId;
            //            var roleId = rolePermission.RoleId;

            //            if (rolePermission.CanCreate)
            //            {
            //                if (!_createCache.ContainsKey(modelClassId))
            //                {
            //                    _createCache.Add(modelClassId, new HashSet<int>());
            //                }

            //                _createCache[modelClassId].Add(roleId);
            //            }

            //            if (rolePermission.CanRead)
            //            {
            //                if (!_readCache.ContainsKey(modelClassId))
            //                {
            //                    _readCache.Add(modelClassId, new HashSet<int>());
            //                }

            //                _readCache[modelClassId].Add(roleId);
            //            }

            //            if (rolePermission.CanUpdate)
            //            {
            //                if (!_updateCache.ContainsKey(modelClassId))
            //                {
            //                    _updateCache.Add(modelClassId, new HashSet<int>());
            //                }

            //                _updateCache[modelClassId].Add(roleId);
            //            }

            //            if (rolePermission.CanDelete)
            //            {
            //                if (!_deleteCache.ContainsKey(modelClassId))
            //                {
            //                    _deleteCache.Add(modelClassId, new HashSet<int>());
            //                }

            //                _deleteCache[modelClassId].Add(roleId);
            //            }
            //        }
            //    }
            //}
        }

        public void RemoveFromCache(int userId)
        {
            HashSet<int> cachedSet;
            _userRoleCache.TryRemove(userId, out cachedSet);
        }


        private readonly static RolePermissionCachingService CachingService = new RolePermissionCachingService();

        public static RolePermissionCachingService GetCurrent()
        {
            return CachingService;
        }
    }
}
