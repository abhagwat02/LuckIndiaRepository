using System;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.ModelFactories
{
    sealed class UserPasswordRequestFactory
    {
        public static UserPasswordRequest CreateNew(int userId, int validHours)
        {
            return new UserPasswordRequest
            {
                UserId = userId,
                EndDate = DateTime.UtcNow.AddHours(validHours),
            };
        }
    }
}
