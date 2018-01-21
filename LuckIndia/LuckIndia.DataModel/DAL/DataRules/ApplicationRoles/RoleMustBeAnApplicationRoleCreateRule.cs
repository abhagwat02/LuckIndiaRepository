using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Enums;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.ApplicationRoles
{
    sealed class RoleMustBeAnApplicationRoleCreateRule : CreateRule<ApplicationRole>
    {
        public RoleMustBeAnApplicationRoleCreateRule(ApplicationRole model)
            : base(model)
        {
        }

        public override void Validate()
        {
            using (var context = CMDDatabaseContext.GetContext())
            {
                if (!context.Roles.Any(x => x.Id == this.Model.RoleId && x.RoleTypeId == (int)RoleTypeValue.Application))
                {
                    throw new InvalidDataException("Role must be an application role.");
                }
            }
        }
    }
}