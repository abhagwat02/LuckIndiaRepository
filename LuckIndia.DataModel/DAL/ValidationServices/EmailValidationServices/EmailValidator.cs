using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Alphaeon.Services.EnterpriseAPI.DAL.ValidationServices.EmailValidationServices
{
    sealed class EmailValidator
    {
        public static bool IsValid(string emailToValidate)
        {
            if (string.IsNullOrWhiteSpace(emailToValidate))
            {
                return false;
            }

            var valid = Regex.IsMatch(emailToValidate, @"^[a-zA-Z0-9_\.\-]+@[a-zA-Z0-9_\.\-]+\.[a-zA-Z]{2,4}$");

            return valid;
        }

        /// <summary>
        /// Checks a string of email addresses seperated by a character.
        /// Returns true only if all addresses are valid.
        /// </summary>
        /// <param name="emailsToValidate">Email addresses to validate.</param>
        /// <param name="separators">Array of separators, typically just a semi-colon, but you may want a space or a new line.</param>
        /// <returns></returns>
        public static bool IsValid(string emailsToValidate, char[] separators)
        {
            if (string.IsNullOrWhiteSpace(emailsToValidate) || separators == null || separators.Length <= 0)
            {
                return false;
            }

            var addresses = emailsToValidate.Split(separators, StringSplitOptions.RemoveEmptyEntries);

            var valid = addresses.All(IsValid);

            return valid;
        }
    }
}
