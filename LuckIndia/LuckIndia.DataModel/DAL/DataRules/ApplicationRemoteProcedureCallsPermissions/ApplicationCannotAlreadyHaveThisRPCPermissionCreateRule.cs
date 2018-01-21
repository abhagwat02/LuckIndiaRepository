using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.ApplicationRemoteProcedureCallsPermissions
{
    sealed class ApplicationCannotAlreadyHaveThisRPCPermissionCreateRule : CreateRule<ApplicationRemoteProcedureCallsPermission>
    {
        public ApplicationCannotAlreadyHaveThisRPCPermissionCreateRule(ApplicationRemoteProcedureCallsPermission model) 
            : base(model)
        {
        }

        public override void Validate()
        {
            using (var context = CMDDatabaseContext.GetContext())
            {
                if (context.ApplicationRemoteProcedureCallsPermissions.Any(x => x.ApplicationId == this.Model.ApplicationId && x.RemoteProcedureCallId == this.Model.RemoteProcedureCallId))
                {
                    throw new InvalidDataException("Application already has this RPC permission.");
                }
            }
        }
    }
}