
using Alphaeon.Services.EnterpriseAPI.DAL.Enums;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Exceptions
{
    sealed class TooManyRequestsException : DataAccessLayerException
    {
        /// <summary>
        /// Generic too many request exception with a severity of error.
        /// </summary>
        public TooManyRequestsException()
            : this("Too many requests")
        { }

        /// <summary>
        /// Too many requests exception with a custom message and a severity of error.
        /// </summary>
        /// <param name="message"></param>
        public TooManyRequestsException(string message)
            : this(message, ExceptionSeverity.Error)
        { }

        /// <summary>
        /// Too many requests exception with a custom message and custom severity.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exceptionSeverity"></param>
        public TooManyRequestsException(string message, ExceptionSeverity exceptionSeverity)
            : base(message)
        {
            this.ExceptionSeverity = exceptionSeverity;
        }
    }
}