

using LuckIndia.DataModel.DAL.Enums;

namespace LuckIndia.APIs.DAL.Exceptions
{
    class InvalidAccessTokenException : DataAccessLayerException
    {
        /// <summary>
        /// Generic invalid access token exception with a severity of warning.
        /// </summary>
        public InvalidAccessTokenException()
            : this("Invalid AccessToken")
        {
        }

        /// <summary>
        /// Invalid access token exception with a custom message and a severity of warning.
        /// </summary>
        /// <param name="message"></param>
        public InvalidAccessTokenException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Invalid access token exception with a custom message and a custom severity.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exceptionSeverity"></param>
        public InvalidAccessTokenException(string message, ExceptionSeverity exceptionSeverity)
            : base(message)
        {
            this.ExceptionSeverity = exceptionSeverity;
        }
    }
}
