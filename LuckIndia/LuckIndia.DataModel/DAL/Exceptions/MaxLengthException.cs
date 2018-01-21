using System.Runtime.CompilerServices;

namespace LuckIndia.APIs.DAL.Exceptions
{
    sealed class MaxLengthException : DataAccessLayerException
    {
        public MaxLengthException(long maxLength, [CallerMemberName] string propertyName = null)
            : base(string.Format("Exceeded maximum length of {0}{1}",
            maxLength,
            (propertyName == null) ? "." : string.Format(" for {0}.", propertyName)))
        {
        }
    }
}
