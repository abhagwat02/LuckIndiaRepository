using LuckIndia.DataModel.DAL.Enums;

namespace LuckIndia.APIs.DAL.Exceptions
{
    class ForbiddenException : DataAccessLayerException
    {
        /// <summary>
        /// Generic forbidden exception with a severity of error.
        /// </summary>
        public ForbiddenException()
            : this("Forbidden operation.")
        {
        }

        /// <summary>
        /// Forbidden exception with a custom message and a severity of error.
        /// </summary>
        /// <param name="message"></param>
        public ForbiddenException(string message)
            : this(message, ExceptionSeverity.Error)
        {
        }

        /// <summary>
        /// Forbidden exception with a custom message and a custom severity.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exceptionSeverity"></param>
        public ForbiddenException(string message, ExceptionSeverity exceptionSeverity)
            : base(message)
        {
            this.ExceptionSeverity = exceptionSeverity;
        }
    }
}
