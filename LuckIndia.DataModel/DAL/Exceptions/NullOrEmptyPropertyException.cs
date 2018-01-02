using System.Runtime.CompilerServices;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Exceptions
{
    sealed class NullOrEmptyPropertyException : DataAccessLayerException
    {
        public NullOrEmptyPropertyException([CallerMemberName] string propertyName = null)
            : base((propertyName == null) ? "Invalid value." : string.Format("Invalid value for {0}.", propertyName))
        {
        }
    }
}
