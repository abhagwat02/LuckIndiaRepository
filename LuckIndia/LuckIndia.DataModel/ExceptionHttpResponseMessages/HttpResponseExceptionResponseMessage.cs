using LuckIndia.Models.DTO;
using System.Net;
using System.Web.Http;


namespace LuckIndia.DataModel.ExceptionHttpResponseMessages

{
    sealed class HttpResponseExceptionResponseMessage : ApiResponseMessage
    {
        private readonly HttpResponseException _exception;

        public HttpResponseExceptionResponseMessage(HttpResponseException exception)
            : base(exception)
        {
            _exception = exception;
        }

        protected override HttpStatusCode GetStatusCode()
        {
            return _exception.Response.StatusCode;
        }

        protected override ApiMessage GetApiMessage()
        {
            var apiMessage = new ApiMessage
            {
                Message = _exception.Response.ReasonPhrase
            };

            return apiMessage;
        }
    }
}
