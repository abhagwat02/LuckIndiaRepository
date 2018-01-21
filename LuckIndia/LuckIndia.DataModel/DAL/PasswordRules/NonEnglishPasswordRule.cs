namespace Alphaeon.Services.EnterpriseAPI.DAL.PasswordRules
{
    sealed class NonEnglishPasswordRule : PasswordRule
    {
        protected override void ValidatePassword(string password)
        {
            //For now everything here is valid.
        }
    }
}