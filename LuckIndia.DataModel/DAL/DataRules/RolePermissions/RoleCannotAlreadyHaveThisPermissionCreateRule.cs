using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.RolePermissions
{
    sealed class RoleCannotAlreadyHaveThisPermissionCreateRule : CreateRule<RolePermission>
    {
        public RoleCannotAlreadyHaveThisPermissionCreateRule(RolePermission model)
            : base(model)
        {
        }

        public override void Validate()
        {
            using (var context = CMDDatabaseContext.GetContext())
            {
                if (context.RolePermissions.Any(x => x.RoleId == this.Model.RoleId && x.ModelClassId == this.Model.ModelClassId))
                {
                    throw new InvalidDataException("Role already has this permission.");
                }
            }
        }
    }
}
