using System;
using System.Runtime.CompilerServices;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;

namespace Alphaeon.Services.EnterpriseAPI.DAL.ValidationServices.UrlValidationServices
{
    sealed class AbsoluteUrlValidator : UrlValidator
    {
        public AbsoluteUrlValidator(string validateMe, [CallerMemberName] string propertyName = null)
            : base(validateMe, propertyName)
        {
        }

        public override void Validate()
        {
            Uri uri;

            if (!Uri.TryCreate(this.Data, UriKind.Absolute, out uri))
            {
                // ReSharper disable once ExplicitCallerInfoArgument
                throw new AbsoluteUrlException(this.PropertyName);
            }
        }
    }
}
