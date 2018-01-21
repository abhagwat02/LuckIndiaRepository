using System;
using LuckIndia.DataModel.DAL.Enums;

namespace LuckIndia.APIs.DAL.Exceptions
{
    sealed class ConflictException : DataAccessLayerException
    {
        public ConflictException(Type modelType, int modelId, string message)
            : base(message, ExceptionSeverity.Information)
        {
            ModelType = modelType;
            ModelId = modelId;
        }

        public Type ModelType { get; private set; }

        public int ModelId { get; private set; }
    }
}
