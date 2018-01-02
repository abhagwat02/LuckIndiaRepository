using System;
using System.Net;

namespace LuckIndia.DataModel.ExceptionHttpResponseMessages

{
    sealed class BadRequestResponseMessage : ApiResponseMessage
    {
        public BadRequestResponseMessage(Exception exception) 
            : base(exception, HttpStatusCode.BadRequest)
        {
        }
    }
}
