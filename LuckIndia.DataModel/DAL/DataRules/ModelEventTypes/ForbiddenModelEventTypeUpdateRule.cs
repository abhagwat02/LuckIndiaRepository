using System.Collections.Generic;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.ModelEventTypes
{
    sealed class ForbiddenModelEventTypeUpdateRule : UpdateRule<ModelEventType>
    {
        public ForbiddenModelEventTypeUpdateRule(ModelEventType model, IDictionary<string, object> delta)
            : base(model, delta)
        {
        }

        public override void Validate()
        {
            throw new ForbiddenException();
        }
    }
}