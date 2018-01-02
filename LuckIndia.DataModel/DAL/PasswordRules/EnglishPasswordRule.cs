using System.Text.RegularExpressions;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;

namespace Alphaeon.Services.EnterpriseAPI.DAL.PasswordRules
{
    sealed class EnglishPasswordRule : PasswordRule
    {
        protected override void ValidatePassword(string password)
        {
            var regex = new Regex("^.*(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z]).*$");
            if (!regex.IsMatch(password))
            {
                throw new InvalidDataException("Password does not meet requirements. Password must contain at least one uppercase letter, one lowercase letter, and one number.");
            }
        }
    }
}