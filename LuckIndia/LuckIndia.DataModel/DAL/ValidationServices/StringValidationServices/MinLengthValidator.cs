using LuckIndia.APIs.DAL.Exceptions;
using System;

namespace LuckIndia.DataModel.DAL.ValidationServices.StringValidationServices
{
    sealed class MinLengthValidator : StringValidator
    {
        private readonly int _minLength;


        public MinLengthValidator(String validateMe, int minLength, string propertyName)
            : base(validateMe, propertyName)
        {
            this._minLength = minLength;
        }


        public override void Validate()
        {
            if (null == Data)
            {
                return;
            }


            if (Data.Length < _minLength)
            {
                throw new MinLengthException(_minLength, this.PropertyName);
            }
        }
    }
}