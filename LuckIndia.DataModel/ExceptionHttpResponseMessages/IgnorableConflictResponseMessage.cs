using System;
using System.Net;

namespace LuckIndia.DataModel.ExceptionHttpResponseMessages

{
    sealed class IgnorableConflictResponseMessage : ApiResponseMessage
    {
        public IgnorableConflictResponseMessage(Exception exception)
            : base(exception, HttpStatusCode.Accepted)
        {
        }
    }
}
