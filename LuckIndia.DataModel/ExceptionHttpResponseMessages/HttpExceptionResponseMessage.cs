using System.Net;
using System.Web;

namespace LuckIndia.DataModel.ExceptionHttpResponseMessages

{
    sealed class HttpExceptionResponseMessage : ApiResponseMessage
    {
        private readonly HttpException _exception;

        public HttpExceptionResponseMessage(HttpException exception)
            : base(exception)
        {
            _exception = exception;
        }

        protected override HttpStatusCode GetStatusCode()
        {
            return (HttpStatusCode)_exception.GetHttpCode();
        }
    }
}
