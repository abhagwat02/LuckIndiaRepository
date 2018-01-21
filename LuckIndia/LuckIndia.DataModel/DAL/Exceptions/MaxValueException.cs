using System.Runtime.CompilerServices;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Exceptions
{
    sealed class MaxValueException : DataAccessLayerException
    {
        public MaxValueException(long maxValue, [CallerMemberName] string propertyName = null)
            : base(string.Format("Exceeded maximum value of {0}{1}",
            maxValue,
            (propertyName == null) ? "." : string.Format(" for {0}.", propertyName)))
        {
        }
    }
}
