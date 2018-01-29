using LuckIndia.Models.DTO;
using System;
using System.Diagnostics;
using System.Net;


namespace LuckIndia.DataModel.ExceptionHttpResponseMessages

{
    sealed class ObjectDisposedResponseMessage : ApiResponseMessage
    {
        public ObjectDisposedResponseMessage(Exception exception)
            : base(exception, HttpStatusCode.InternalServerError)
        {
            //WindowsEventLogger.LogException(exception, EventCategory.DataAccessLayer, EventLogEntryType.Error);
           // CMDLogger.LogException(exception, null, Convert.ToString(ALPHAEON.CMD.Common.Enums.Components.EnterpriseAPI), new StackTrace(exception).GetFrame(0).GetMethod().Name);
        }

        protected override ApiMessage GetApiMessage()
        {
            return new ApiMessage
            {
                Message = "Internal error. Possible ManyToManyObject attribute missing."
            };
        }
    }
}
