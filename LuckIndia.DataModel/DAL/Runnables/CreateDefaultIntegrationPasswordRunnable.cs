using Alphaeon.Services.EnterpriseAPI.ApplicationServices.HashingServices;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;
using System;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    sealed class CreateDefaultIntegrationPasswordRunnable : IRunnable
    {
        private readonly User _user;

        public CreateDefaultIntegrationPasswordRunnable(User user)
        {
            _user = user;
        }


        public void Execute(CMDDatabaseContext context)
        {
            var hashedPassword = HashingService.Hash(GenerateDefaultPassword(), _user.PasswordSalt);
            _user.PasswordHash = hashedPassword;
            context.SaveChanges();
        }


        private string GenerateDefaultPassword()
        {
            try
            {//bt112687
                var firstInitial = _user.FirstName.ToLower()[0];
                var lastInitial = _user.LastName.ToLower()[0];
                var dobString = _user.DateOfBirth.Value.ToString("MMddyy");

                return String.Format("{0}{1}{2}", firstInitial, lastInitial, dobString);
            }
            catch (Exception)
            {
                throw new InvalidDataException("User object does not contain sufficient information to create a default integration password.");
            }
        }

        public T Execute<T>(CMDDatabaseContext context) where T : class
        {
            throw new NotImplementedException();
        }
    }
}