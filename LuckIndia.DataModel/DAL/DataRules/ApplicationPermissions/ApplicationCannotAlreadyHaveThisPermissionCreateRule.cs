using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.ApplicationPermissions
{
    sealed class ApplicationCannotAlreadyHaveThisPermissionCreateRule : CreateRule<ApplicationPermission>
    {
        public ApplicationCannotAlreadyHaveThisPermissionCreateRule(ApplicationPermission model)
            : base(model)
        {
        }

        public override void Validate()
        {
            using (var context = CMDDatabaseContext.GetContext())
            {
                if (context.ApplicationPermissions.Any(x => x.ApplicationId == this.Model.ApplicationId && x.ModelClassId == this.Model.ModelClassId))
                {
                    throw new InvalidDataException("Application already has this permission.");
                }
            }
        }
    }
}