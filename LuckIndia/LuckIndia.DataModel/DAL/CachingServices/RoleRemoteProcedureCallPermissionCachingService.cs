using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Alphaeon.Services.EnterpriseAPI.Interfaces;

namespace Alphaeon.Services.EnterpriseAPI.DAL.CachingServices
{
    public class RoleRemoteProcedureCallPermissionCachingService : IRefreshable
    {
        private readonly object _lockObject = new object();

        private readonly Dictionary<int, HashSet<int>> _accessCache = new Dictionary<int, HashSet<int>>();
      
        private readonly ConcurrentDictionary<int, HashSet<int>> _userRoleCache = new ConcurrentDictionary<int, HashSet<int>>();

        private RoleRemoteProcedureCallPermissionCachingService()
        {
            this.Refresh();
        }

        public bool CanAccess(int rpcId, CMDDatabaseContext context)
        {
            if (!_accessCache.ContainsKey(rpcId))
            {
                return false;
            }

            // var userRoles = GetUserRoleIds(userId);
            var userRoles = context.SecurityContext.User.Roles.Select(x => x.Id).ToList();
            return _accessCache[rpcId].Intersect(userRoles).Any();
        }

     
        public void Refresh()
        {
            lock (_lockObject)
            {
                _accessCache.Clear();
            
                using (var context = CMDDatabaseContext.GetContext())
                {
                    foreach (var roleRPCPermission in context.RoleRemoteProcedureCallsPermissions)
                    {
                        var rpcId = roleRPCPermission.RemoteProcedureCallId;
                        var roleId = roleRPCPermission.RoleId;

                        if (roleRPCPermission.CanAccess)
                        {
                            if (!_accessCache.ContainsKey(rpcId))
                            {
                                _accessCache.Add(rpcId, new HashSet<int>());
                            }

                            _accessCache[rpcId].Add(roleId);
                        }

                    
                    }
                }
            }
        }

        public void RemoveFromCache(int userId)
        {
            HashSet<int> cachedSet;
            _userRoleCache.TryRemove(userId, out cachedSet);
        }


        private readonly static RoleRemoteProcedureCallPermissionCachingService CachingService = new RoleRemoteProcedureCallPermissionCachingService();

        public static RoleRemoteProcedureCallPermissionCachingService GetCurrent()
        {
            return CachingService;
        }
    }
}
