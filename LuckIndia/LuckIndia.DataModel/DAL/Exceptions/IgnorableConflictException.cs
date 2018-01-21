using Alphaeon.Services.EnterpriseAPI.DAL.Enums;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Exceptions
{
    sealed class IgnorableConflictException : DataAccessLayerException
    {
        public IgnorableConflictException(string message)
            : base(message, ExceptionSeverity.Information)
        {
        }
    }
}
