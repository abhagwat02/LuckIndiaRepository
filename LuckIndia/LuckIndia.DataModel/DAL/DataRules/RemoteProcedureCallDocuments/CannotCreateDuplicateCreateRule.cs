using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.RemoteProcedureCallDocuments
{
    sealed class CannotCreateDuplicateCreateRule : CreateRule<RemoteProcedureCallDocument>
    {
        public CannotCreateDuplicateCreateRule(RemoteProcedureCallDocument model)
            : base(model)
        {
        }

        public override void Validate()
        {
            using (var context = CMDDatabaseContext.GetContext())
            {
                if (context.RemoteProcedureCallDocuments.Any(rd => rd.RemoteProcedureCallId == this.Model.RemoteProcedureCallId && rd.Verb.Equals(this.Model.Verb)))
                {
                    throw new InvalidDataException("This rpc already has documentation for the specified verb.");
                }
            }
        }
    }
}