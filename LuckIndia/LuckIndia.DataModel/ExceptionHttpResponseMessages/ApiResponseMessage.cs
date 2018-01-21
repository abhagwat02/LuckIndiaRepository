using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using LuckIndia.APIs.DTO;
using LuckIndia.DataModel.Enums;

namespace LuckIndia.DataModel.ExceptionHttpResponseMessages
{
    abstract class ApiResponseMessage
    {
        private readonly string _message = "";

        private readonly HttpStatusCode _statusCode;

        protected ApiResponseMessage(Exception exception)
            : this(exception, HttpStatusCode.BadRequest)
        {
        }

        public static JsonSerializerSettings StandardJsonSerializerSettings = new JsonSerializerSettings
        {
            DateTimeZoneHandling = DateTimeZoneHandling.Utc,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        };
        protected ApiResponseMessage(Exception exception, HttpStatusCode statusCode)
        {
            if (null != exception)
            {
                _message = exception.Message;
            }

            _statusCode = statusCode;
        }

        protected virtual ApiMessage GetApiMessage()
        {
            return new ApiMessage { Message = _message };
        }

        protected virtual HttpStatusCode GetStatusCode()
        {
            return _statusCode;
        }

        protected virtual void SetCustomHeader(HttpResponseMessage responseMessage)
        {
            //default is not going to add any headers.
        }

        public HttpResponseMessage GetHttpResponseMessage()
        {
            var apiMessage = this.GetApiMessage();

            var json = JsonConvert.SerializeObject(apiMessage, StandardJsonSerializerSettings);


            var responseMessage = new HttpResponseMessage(this.GetStatusCode())
            {
                Content = new StringContent(json)
            };

            responseMessage.Content.Headers.ContentType = new MediaTypeHeaderValue(MediaTypeValue.JSON);

            this.SetCustomHeader(responseMessage);

            return responseMessage;
        }
    }
}
