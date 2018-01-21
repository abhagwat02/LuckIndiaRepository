using System;
using System.Linq;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    sealed class CreateOrGetUserRunnable : IRunnable
    {
        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _email;

        /// <summary>
        /// Gets the user if the email exists, otherwise creates the user and returns it.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        public CreateOrGetUserRunnable(string firstName, string lastName, string email)
        {
            _firstName = firstName;
            _lastName = lastName;
            _email = email ?? string.Empty;
        }

        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public T Execute<T>(CMDDatabaseContext context) where T : class
        {
            var user = context.Users.FirstOrDefault(x => x.Email == _email);
            if (user != null)
            {
                return user as T;
            }

            var createUser = new CreateUserRunnable(_firstName, _lastName, _email);
            return context.ExecuteRunnable<T>(createUser);
        }
    }
}
