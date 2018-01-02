using System.Collections.Generic;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.ModelClassDocuments
{
    sealed class VerbCannotBeChangedUpdateRule : UpdateRule<ModelClassDocument>
    {
        public VerbCannotBeChangedUpdateRule(ModelClassDocument model, IDictionary<string, object> delta)
            : base(model, delta)
        {
        }

        public override void Validate()
        {
            var verbKey = DAL.PropertyName<ModelClassDocument>(x => x.Verb);
            if (this.Delta.ContainsKey(verbKey))
            {
                throw new InvalidDataException("You cannot change the verb.");
            }
        }
    }
}