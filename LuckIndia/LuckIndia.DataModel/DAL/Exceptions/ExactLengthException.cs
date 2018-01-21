using System.Runtime.CompilerServices;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Exceptions
{
    sealed class ExactLengthException: DataAccessLayerException
    {
        public ExactLengthException(long value, [CallerMemberName] string propertyName = null)
            :base(string.Format("Required length of {0}{1}",
            value,
            (propertyName == null) ? "." : string.Format(" for {0}.", propertyName)))
        {
        }
    }
}