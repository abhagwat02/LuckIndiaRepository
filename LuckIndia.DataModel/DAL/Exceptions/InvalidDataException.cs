using Alphaeon.Services.EnterpriseAPI.DAL.Enums;
using System;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Exceptions
{
    [Serializable]
    class InvalidDataException : DataAccessLayerException
    {
        /// <summary>
        /// Generic invalid data exception with the severity of warning.
        /// </summary>
        public InvalidDataException()
            : this("Invalid data.")
        {
        }

        /// <summary>
        /// Invalid data exception with a custom message and the severity of warning.
        /// </summary>
        /// <param name="message"></param>
        public InvalidDataException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Invalid data exception with a custom message and custom severity.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exceptionSeverity"></param>
        public InvalidDataException(string message, ExceptionSeverity exceptionSeverity)
            : base(message)
        {
            this.ExceptionSeverity = exceptionSeverity;
        }
    }
}
