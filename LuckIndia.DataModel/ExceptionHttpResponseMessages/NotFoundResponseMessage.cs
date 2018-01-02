using System;
using System.Net;

namespace LuckIndia.DataModel.ExceptionHttpResponseMessages

{
    sealed class NotFoundResponseMessage : ApiResponseMessage
    {
        public NotFoundResponseMessage(Exception exception)
            : base(exception, HttpStatusCode.NotFound)
        {
        }
    }
}
