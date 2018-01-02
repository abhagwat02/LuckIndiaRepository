using LuckIndia.DataModel.DAL.Enums;

namespace LuckIndia.APIs.DAL.Exceptions
{
    class ModelNotFoundException : DataAccessLayerException
    {
        /// <summary>
        /// Generic model not found exception with a severity of warning.
        /// </summary>
        public ModelNotFoundException()
            : base("Object not found.")
        {
        }

        /// <summary>
        /// Model not found exception with a custom message and a severity of warning.
        /// </summary>
        /// <param name="message"></param>
        public ModelNotFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Model not found exception with a custom message and a custom severity.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exceptionSeverity"></param>
        public ModelNotFoundException(string message, ExceptionSeverity exceptionSeverity)
            : base(message)
        {
            this.ExceptionSeverity = exceptionSeverity;
        }
    }
}
