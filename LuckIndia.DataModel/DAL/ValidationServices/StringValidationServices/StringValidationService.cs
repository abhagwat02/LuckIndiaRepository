using System;
using System.Runtime.CompilerServices;

namespace LuckIndia.DataModel.DAL.ValidationServices.StringValidationServices
{
    sealed class StringValidationService
    {
        private readonly String _data;
        private readonly string _propertyName;

        public StringValidationService(String validateMe, [CallerMemberName] string propertyName = null)
        {
            _data = validateMe;
            _propertyName = propertyName;
        }


        public StringValidationService ValidateMaxLength(int maxLength)
        {
            new MaxLengthValidator(_data, maxLength, _propertyName).Validate();
            return this;
        }


        public StringValidationService ValidateMinLength(int minLength)
        {
            new MinLengthValidator(_data, minLength, _propertyName).Validate();
            return this;
        }


        public StringValidationService ValidateExactLength(int length)
        {
            try
            {
                new MinLengthValidator(_data, length, _propertyName).Validate();
                new MaxLengthValidator(_data, length, _propertyName).Validate();
            }
            catch
            {
               // throw new ExactLengthException(length);
            }

            return this;
        }


        public StringValidationService ValidateNotNull()
        {
            //new NotNullValidator(_data, _propertyName).Validate();
            return this;
        }


        public StringValidationService ValidateNotNullOrWhiteSpace()
        {
            //new NotNullOrWhiteSpaceValidator(_data, _propertyName).Validate();
            return this;
        }
    }
}
