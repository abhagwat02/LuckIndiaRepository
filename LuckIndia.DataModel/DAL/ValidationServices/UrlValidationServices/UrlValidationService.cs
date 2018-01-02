using System;
using System.Runtime.CompilerServices;
using Alphaeon.Services.EnterpriseAPI.DAL.ValidationServices.StringValidationServices;

namespace Alphaeon.Services.EnterpriseAPI.DAL.ValidationServices.UrlValidationServices
{
    public class UrlValidationService
    {
        private readonly string _data;
        private readonly string _propertyName;

        public UrlValidationService(String validateMe, [CallerMemberName] string propertyName = null)
        {
            _data = validateMe;
            _propertyName = propertyName;
        }

        private void ValidateMaxLength()
        {
            // ReSharper disable once ExplicitCallerInfoArgument
            new BrowserMaxLengthValidator(_data, _propertyName).Validate();
        }

        private void ValidateNotNullOrWhiteSpace()
        {
            new NotNullOrWhiteSpaceValidator(_data, _propertyName).Validate();
        }

        /// <summary>
        /// Validates the url is not null, meets the browser length, and is absolute.
        /// </summary>
        public UrlValidationService ValidateAbsoluteUrl()
        {
            this.ValidateNotNullOrWhiteSpace();
            this.ValidateMaxLength();

            // ReSharper disable once ExplicitCallerInfoArgument
            new AbsoluteUrlValidator(_data, _propertyName).Validate();
            return this;
        }

        /// <summary>
        /// Validates the url is not null, meets the browser length, and is relative.
        /// </summary>
        public UrlValidationService ValidateRelativeUrl()
        {
            this.ValidateNotNullOrWhiteSpace();
            this.ValidateMaxLength();

            // ReSharper disable once ExplicitCallerInfoArgument
            new RelativeUrlValidator(_data, _propertyName).Validate();
            return this;
        }
    }
}
