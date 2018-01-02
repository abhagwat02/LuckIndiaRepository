using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Enums;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.ApplicationRemoteProcedureCallsPermissions
{
    sealed class BaseApplicationCannotAlreadyHavePermissionCreateRule : CreateRule<ApplicationRemoteProcedureCallsPermission>
    {
        public BaseApplicationCannotAlreadyHavePermissionCreateRule(ApplicationRemoteProcedureCallsPermission model)
            : base(model)
        {
        }

        public override void Validate()
        {
            using (var context = CMDDatabaseContext.GetContext())
            {
                const int baseApplicationId = (int)ApplicationValue.BasePermissions;
                if (context.ApplicationRemoteProcedureCallsPermissions.Any(x => x.ApplicationId == baseApplicationId && x.RemoteProcedureCallId == this.Model.RemoteProcedureCallId))
                {
                    throw new InvalidDataException("Base application already has this RPC permission.");
                }
            }
        }
    }
}