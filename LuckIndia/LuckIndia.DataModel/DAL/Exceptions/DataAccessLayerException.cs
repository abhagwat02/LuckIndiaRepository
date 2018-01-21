using LuckIndia.DataModel.DAL.Enums;
using System;

namespace LuckIndia.APIs.DAL.Exceptions
{
    abstract class DataAccessLayerException : Exception
    {
        protected DataAccessLayerException() { }

        private ExceptionSeverity _exceptionSeverity = ExceptionSeverity.Warning;

        public ExceptionSeverity ExceptionSeverity
        {
            get { return _exceptionSeverity; }
            set { _exceptionSeverity = value; }
        }

        protected DataAccessLayerException(string message) : base(message) { }

        protected DataAccessLayerException(string message, ExceptionSeverity exceptionSeverity)
            : base(message)
        {
            this.ExceptionSeverity = exceptionSeverity;
        }
    }
}
