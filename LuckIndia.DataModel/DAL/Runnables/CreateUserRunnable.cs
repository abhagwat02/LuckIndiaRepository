using Alphaeon.Services.EnterpriseAPI.Models;
using System;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    sealed class CreateUserRunnable : IRunnable
    {
        private readonly string _firstName;
        private readonly string _lastName;
        private readonly string _email;

        /// <summary>
        /// Creates a user with the given information.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="email"></param>
        public CreateUserRunnable(string firstName, string lastName, string email)
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
            var user = new User
            {
                FirstName = _firstName,
                LastName = _lastName,
                Email = _email,
            };

            return context.Create(user, false) as T;
        }
    }
}
