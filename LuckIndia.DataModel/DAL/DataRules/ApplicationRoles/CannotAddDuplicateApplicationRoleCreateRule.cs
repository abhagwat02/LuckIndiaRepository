using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.ApplicationRoles
{
    sealed class CannotAddDuplicateApplicationRoleCreateRule : CreateRule<ApplicationRole>
    {
        public CannotAddDuplicateApplicationRoleCreateRule(ApplicationRole model) 
            : base(model)
        {
        }

        public override void Validate()
        {
            using (var context = CMDDatabaseContext.GetContext())
            {
                if (context.ApplicationRoles.Any(x => x.ApplicationId == this.Model.ApplicationId && x.RoleId == this.Model.RoleId))
                {
                    throw new InvalidDataException("Application already has this role.");
                }
            }
        }
    }
}