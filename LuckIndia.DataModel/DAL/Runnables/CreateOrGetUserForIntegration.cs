using Alphaeon.Services.EnterpriseAPI.DAL.Enums;
using Alphaeon.Services.EnterpriseAPI.Models;
using System;
using System.Globalization;
using System.Linq;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    sealed class CreateOrGetUserForIntegration : IRunnable
    {
        private readonly string _email;
        private readonly string _firstname;
        private readonly string _lastname;
        private readonly DateTime _dateOfBirth;


        public CreateOrGetUserForIntegration(string email, string firstname, string lastname, string dob)
        {
            if (String.IsNullOrWhiteSpace(firstname) || String.IsNullOrWhiteSpace(lastname) || String.IsNullOrWhiteSpace(dob))
            {
                throw new ArgumentException("Invalid arguments passed to the CreateOrGetUserForIntegration runnable constructor. Valid firstname, lastname and dob required.");
            }


            _email = email;
            _firstname = firstname;
            _lastname = lastname;
            _dateOfBirth = DateTime.ParseExact(dob, "yyyyMMdd", CultureInfo.InvariantCulture);
        }


        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }


        public T Execute<T>(CMDDatabaseContext context) where T : class
        {
            User user = null;

            if (!String.IsNullOrWhiteSpace(_email))
            {
                //I dont want a user to be matched because he had a null or white space email shared with some one who shared 
                //their birthdate.
                user = this.FindUserByEmailAndDob(context);
            }

            if (null == user)
            {
                user = this.FindUserByFirstAndLastAndDob(context);
            }

            if (null == user)
            {
                user = this.CreateBrandNewUser(context);
            }

            return user as T;
        }


        private User FindUserByEmailAndDob(CMDDatabaseContext context)
        {
            return context.Users.Where(u => String.Compare(u.Email, _email) == 0 &&
                u.DateOfBirth.HasValue &&
                DateTime.Compare(u.DateOfBirth.Value, _dateOfBirth) == 0).FirstOrDefault();
        }


        private User FindUserByFirstAndLastAndDob(CMDDatabaseContext context)
        {
            return context.Users.Where(u => String.Compare(u.FirstName, _firstname) == 0 &&
                String.Compare(u.LastName, _lastname) == 0 &&
                u.DateOfBirth.HasValue && DateTime.Compare(u.DateOfBirth.Value, _dateOfBirth) == 0).FirstOrDefault();
        }


        private User CreateBrandNewUser(CMDDatabaseContext context)
        {
            var newUser = new User
            {
                FirstName = _firstname,
                LastName = _lastname,
                DateOfBirth = _dateOfBirth,
                Email = "",
            };


            if (!String.IsNullOrWhiteSpace(_email) && !context.Users.Any(u => String.Compare(u.Email, _email) == 0))
            {
                newUser.Email = _email;
            }

            var newlyCreatedUser = context.Create<User>(newUser, false);

            try
            {
                var passwordGenerator = new CreateDefaultIntegrationPasswordRunnable(newlyCreatedUser);
                passwordGenerator.Execute(context);
            }
            catch (Exception ex)
            {
                DAL.LogException(ex, LogCategory.Runnable);
            }


            return newlyCreatedUser;
        }
    }
}