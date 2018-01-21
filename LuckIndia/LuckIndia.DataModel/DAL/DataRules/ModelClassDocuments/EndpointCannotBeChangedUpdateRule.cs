using System.Collections.Generic;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.ModelClassDocuments
{
    sealed class EndpointCannotBeChangedUpdateRule : UpdateRule<ModelClassDocument>
    {
        public EndpointCannotBeChangedUpdateRule(ModelClassDocument model, IDictionary<string, object> delta)
            : base(model, delta)
        {
        }

        public override void Validate()
        {
            var endpointKey = DAL.PropertyName<ModelClassDocument>(x => x.EndPoint);
            if (this.Delta.ContainsKey(endpointKey))
            {
                throw new InvalidDataException("You cannot change the endpoint.");
            }
        }
    }
}
