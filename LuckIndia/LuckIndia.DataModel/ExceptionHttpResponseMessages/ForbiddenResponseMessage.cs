using System;
using System.Net;

namespace LuckIndia.DataModel.ExceptionHttpResponseMessages

{
    sealed class ForbiddenResponseMessage : ApiResponseMessage
    {
        public ForbiddenResponseMessage(Exception exception)
            : base(exception, HttpStatusCode.Forbidden)
        {
        }
    }
}
