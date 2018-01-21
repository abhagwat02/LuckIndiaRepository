namespace Alphaeon.Services.EnterpriseAPI.DAL.PasswordRules
{
    abstract class PasswordRule
    {
        public void Validate(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
            {
                throw new Exceptions.MinLengthException(8, "Password");
            }

            ValidatePassword(password);
        }

        protected abstract void ValidatePassword(string password);
    }
}