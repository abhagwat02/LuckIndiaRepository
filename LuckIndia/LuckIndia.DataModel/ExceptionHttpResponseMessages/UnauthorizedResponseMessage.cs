using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace LuckIndia.DataModel.ExceptionHttpResponseMessages
{
    internal class UnauthorizedResponseMessage : ApiResponseMessage
    {
        public UnauthorizedResponseMessage(Exception exception)
            : base(exception, HttpStatusCode.Unauthorized)
        {
        }

        public UnauthorizedResponseMessage()
            : this(new Exception("Authorization has been denied for this request."))
        {

        }

        protected override void SetCustomHeader(HttpResponseMessage responseMessage)
        {
            //responseMessage.Headers.WwwAuthenticate.Add(new AuthenticationHeaderValue(WebApiConfig.AUTHORIZATION_HEADER_SCHEME));
        }
    }
}
