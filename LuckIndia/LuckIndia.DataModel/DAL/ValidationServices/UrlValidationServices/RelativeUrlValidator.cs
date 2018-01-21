using System;
using System.Runtime.CompilerServices;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;

namespace Alphaeon.Services.EnterpriseAPI.DAL.ValidationServices.UrlValidationServices
{
    sealed class RelativeUrlValidator : UrlValidator
    {
        public RelativeUrlValidator(string validateMe, [CallerMemberName] string propertyName = null)
            : base(validateMe, propertyName)
        {
        }

        public override void Validate()
        {
            Uri uri;

            if (!Uri.TryCreate(this.Data, UriKind.Relative, out uri))
            {
                // ReSharper disable once ExplicitCallerInfoArgument
                throw new RelativeUrlException(this.PropertyName);
            }
        }
    }
}
