using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.DataRules.ModelEventTypes
{
    sealed class ForbiddenModelEventTypeCreateRule : CreateRule<ModelEventType>
    {
        public ForbiddenModelEventTypeCreateRule(ModelEventType model) : base(model)
        {
        }

        public override void Validate()
        {
            throw new ForbiddenException();
        }
    }
}