using System.Runtime.CompilerServices;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Exceptions
{
    sealed class AbsoluteUrlException : DataAccessLayerException
    {
        public AbsoluteUrlException([CallerMemberName] string propertyName = null)
            : base((propertyName == null) ? "Invalid absolute url." : string.Format("Invalid absolute url for {0}.", propertyName))
        {
        }
    }
}
