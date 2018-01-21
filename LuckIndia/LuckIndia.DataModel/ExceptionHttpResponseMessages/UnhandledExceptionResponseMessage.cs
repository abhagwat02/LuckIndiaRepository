using System;
using System.Diagnostics;
using System.Net;

namespace LuckIndia.DataModel.ExceptionHttpResponseMessages

{
    class UnhandledExceptionResponseMessage : ApiResponseMessage
    {
        public UnhandledExceptionResponseMessage(Exception exception)
            : base(exception, HttpStatusCode.InternalServerError)
        {
            //CMDApiLogger.LogException(exception, EventCategory.DataAccessLayer, EventLogEntryType.Error);
        }

    }
}
