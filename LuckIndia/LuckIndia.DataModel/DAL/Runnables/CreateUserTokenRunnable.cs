using Alphaeon.Services.EnterpriseAPI.ApplicationServices.HashingServices;
using Alphaeon.Services.EnterpriseAPI.DAL.Enums;
using Alphaeon.Services.EnterpriseAPI.DAL.Exceptions;
using Alphaeon.Services.EnterpriseAPI.Models;
using System;

namespace Alphaeon.Services.EnterpriseAPI.DAL.Runnables
{
    sealed class CreateUserTokenRunnable : IRunnable
    {
        private readonly int _userId;
        private readonly string _authorityApplicationToken;
        private readonly string _applicationToken;
        private readonly int _validDays;

        /// <summary>
        /// Creates a UserToken for the given user.
        /// </summary>
        /// <param name="applicationToken"></param>
        /// <param name="userId">Id of the user.</param>
        /// <param name="authorityApplicationToken"></param>
        /// <param name="validDays">Days the token valid for. Must be 1 or more.</param>
        public CreateUserTokenRunnable(string authorityApplicationToken, string applicationToken, int userId, int validDays = 30)
        {
            _userId = userId;
            _authorityApplicationToken = authorityApplicationToken;
            _applicationToken = applicationToken;
            _validDays = validDays;

            if (_validDays < 1)
            {
                throw new MinValueException(1);
            }
        }

        public void Execute(CMDDatabaseContext context)
        {
            throw new NotImplementedException();
        }

        public T Execute<T>(CMDDatabaseContext context) where T : class
        {
            context.ValidateApplicationToken(_applicationToken);

            try
            {
                context.ValidateAuthorityApplicationToken(_authorityApplicationToken);
            }
            catch (Exception)
            {
                var message = string.Format("Legacy call to create a UserToken without an authority was used.{0}Application: {1}{0}User: {2}",
                        Environment.NewLine,
                        _applicationToken,
                        _userId);

                DAL.LogException(new Exception(message), LogCategory.Runnable);
            }

            var token = HashingService.GenerateToken();

            var userToken = new UserToken
            {
                UserId = _userId,
                Token = token,
            };

            userToken.EndDate = userToken.StartDate.AddDays(_validDays);

            return context.Create(userToken, false) as T;
        }
    }
}
