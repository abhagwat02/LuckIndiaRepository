using System.Runtime.CompilerServices;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Exceptions
{
    sealed class MinValueException : DataAccessLayerException
    {
        public MinValueException(long minValue, [CallerMemberName] string propertyName = null)
            : base(string.Format("Exceeded minimum value of {0}{1}",
            minValue,
            (propertyName == null) ? "." : string.Format(" for {0}.", propertyName)))
        {
        }
    }
}
