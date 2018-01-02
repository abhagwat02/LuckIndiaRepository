using System.Runtime.CompilerServices;

namespace LuckIndia.APIs.DAL.Exceptions
{
    sealed class MinLengthException : DataAccessLayerException
    {
        public MinLengthException(long minValue, [CallerMemberName] string propertyName = null)
            : base(string.Format("Required minimum length of {0}{1}",
            minValue,
            (propertyName == null) ? "." : string.Format(" for {0}.", propertyName)))
        {
        }
    }
}
