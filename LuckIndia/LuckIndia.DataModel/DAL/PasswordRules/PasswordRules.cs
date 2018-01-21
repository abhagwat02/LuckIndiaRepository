using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Alphaeon.Services.EnterpriseAPI.DAL.PasswordRules
{
    sealed class PasswordRules
    {
        /// <summary>
        /// Validates the given plain text password against all rules.
        /// </summary>
        /// <param name="password"></param>
        public static void ValidatePassword(string password)
        {
            foreach (var rule in GetPasswordRules(password))
            {
                rule.Validate(password);
            }
        }

        private static IEnumerable<PasswordRule> GetPasswordRules(string password)
        {
            var rules = new List<PasswordRule>();

            if (Regex.IsMatch(password, "[^\x00-\x7F]"))
            {
                rules.Add(new NonEnglishPasswordRule());
            }
            else
            {
                rules.Add(new EnglishPasswordRule());
            }

            //todo: add more password rules as needed

            return rules;
        }
    }
}
