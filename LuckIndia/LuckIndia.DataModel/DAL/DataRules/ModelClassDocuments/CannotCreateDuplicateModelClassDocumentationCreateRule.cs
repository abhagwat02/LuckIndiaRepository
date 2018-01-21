using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.ModelClassDocuments
{
    sealed class CannotCreateDuplicateModelClassDocumentationCreateRule : CreateRule<ModelClassDocument>
    {
        public CannotCreateDuplicateModelClassDocumentationCreateRule(ModelClassDocument model)
            : base(model)
        {
        }

        public override void Validate()
        {
            using (var context = CMDDatabaseContext.GetContext()) 
            {
                if (context.ModelClassDocuments.Any(md => md.ModelClassId == this.Model.ModelClassId && md.Verb.Equals(this.Model.Verb)))
                {
                    throw new InvalidDataException("This modelclass already has documentation for the specified verb.");
                }
            }
        }
    }
}