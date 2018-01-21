using Alphaeon.Services.EnterpriseAPI.DAL.CachingServices;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Enums;
using System;
using System.Linq;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    /// <summary>
    /// Checks the permission for the given RpcId and ApplicationId.
    /// Throws if application does not have access.
    /// </summary>
    sealed class CheckApplicationRemoteProcedureCallsPermissionRunnable : IRunnable
    {
        private readonly int _rpcId;

        public CheckApplicationRemoteProcedureCallsPermissionRunnable(int rpcId)
        {
            _rpcId = rpcId;
        }

        public void Execute(CMDDatabaseContext context)
        {
            
            if (!RoleRemoteProcedureCallPermissionCachingService.GetCurrent().CanAccess(_rpcId,context))
            {
                throw new ForbiddenException("No permission to access the RPC : "+_rpcId.ToString());
            }
        }

        public T Execute<T>(CMDDatabaseContext context) where T : class
        {
            throw new NotImplementedException();
        }
    }
}