using System.Runtime.CompilerServices;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Exceptions
{
    sealed class RelativeUrlException : DataAccessLayerException
    {
        public RelativeUrlException([CallerMemberName] string propertyName = null)
            : base((propertyName == null) ? "Invalid relative url." : string.Format("Invalid relative url for {0}.", propertyName))
        {
        }
    }
}
