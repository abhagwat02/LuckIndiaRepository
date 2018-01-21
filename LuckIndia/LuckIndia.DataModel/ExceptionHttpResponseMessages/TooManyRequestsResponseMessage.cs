using System;
using System.Diagnostics;
using System.Net;


namespace LuckIndia.DataModel.ExceptionHttpResponseMessages

{
    class TooManyRequestsResponseMessage : ApiResponseMessage
    {
        public TooManyRequestsResponseMessage(Exception exception)
            : base(exception, (HttpStatusCode)429)
        {
            //CMDApiLogger.LogException(exception, EventCategory.DataAccessLayer, EventLogEntryType.Error);
        }
    }
}
