using LuckIndia.Models.DTO;
using System;
using System.Net;

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
