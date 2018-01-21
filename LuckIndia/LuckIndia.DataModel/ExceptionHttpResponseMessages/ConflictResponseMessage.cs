using System;
using System.Net;
using LuckIndia.APIs.DAL.Exceptions;
using LuckIndia.APIs.DTO;

namespace LuckIndia.DataModel.ExceptionHttpResponseMessages

{
    sealed class ConflictResponseMessage : ApiResponseMessage
    {
        private readonly ConflictException _conflictException;

        public ConflictResponseMessage(Exception exception)
            : base(exception, HttpStatusCode.Conflict)
        {
            _conflictException = exception as ConflictException;
        }

        protected override ApiMessage GetApiMessage()
        {
            var apiMessage = base.GetApiMessage();

            if (null == _conflictException)
            {
                return apiMessage;
            }

            var data = new
            {
                Type = _conflictException.ModelType.Name,
                Id = _conflictException.ModelId
            };

            apiMessage.Data = data;

            return apiMessage;
        }
    }
}
