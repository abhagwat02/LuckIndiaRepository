using System;
using System.Runtime.CompilerServices;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;

namespace Alphaeon.Services.EnterpriseAPI.DAL.ValidationServices.UrlValidationServices
{
    /// <summary>
    /// To support IE browsers, the url length must be less than 
    /// 2084 characters with the path length no more than 2048.
    /// </summary>
    sealed class BrowserMaxLengthValidator : UrlValidator
    {
        public BrowserMaxLengthValidator(string validateMe, [CallerMemberName] string propertyName = null)
            : base(validateMe, propertyName)
        {
        }

        public override void Validate()
        {
            Uri url;
            if (Uri.TryCreate(this.Data, UriKind.Absolute, out url))
            {
                if (url.ToString().Length > 2083)
                {
                    // ReSharper disable once ExplicitCallerInfoArgument
                    throw new MaxLengthException(2083, this.PropertyName);
                }

                if (url.PathAndQuery.Length > 2048)
                {
                    // ReSharper disable once ExplicitCallerInfoArgument
                    throw new MaxLengthException(2048, this.PropertyName);
                }

                return;
            }

            if (!Uri.TryCreate(this.Data, UriKind.Relative, out url))
            {
                return;
            }

            if (this.Data.Length > 2048)
            {
                // ReSharper disable once ExplicitCallerInfoArgument
                throw new MaxLengthException(2048, this.PropertyName);
            }
        }
    }
}
