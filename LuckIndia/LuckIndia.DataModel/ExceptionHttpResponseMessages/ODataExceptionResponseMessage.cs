using System;
using System.Net;
using LuckIndia.APIs.DTO;

namespace LuckIndia.DataModel.ExceptionHttpResponseMessages

{
    sealed class ODataExceptionResponseMessage : ApiResponseMessage
    {
        public ODataExceptionResponseMessage(Exception exception)
            : base(exception, HttpStatusCode.BadRequest)
        {
        }

        protected override ApiMessage GetApiMessage()
        {
            return new ApiMessage
            {
                Message = "Invalid OData."
            };
        }
    }
}
