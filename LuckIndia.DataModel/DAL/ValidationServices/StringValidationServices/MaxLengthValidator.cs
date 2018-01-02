using LuckIndia.APIs.DAL.Exceptions;
using System;

namespace LuckIndia.DataModel.DAL.ValidationServices.StringValidationServices
{
    sealed class MaxLengthValidator : StringValidator
    {
        private readonly int _maxLength;

        public MaxLengthValidator(String validateMe, int maxLength, string propertyName)
            : base(validateMe, propertyName)
        {
            this._maxLength = maxLength;
        }


        public override void Validate()
        {
            if (null == Data)
            {
                return;
            }


            if (Data.Length > _maxLength)
            {
                throw new MaxLengthException(100, this.PropertyName);
            }
        }
    }
}
