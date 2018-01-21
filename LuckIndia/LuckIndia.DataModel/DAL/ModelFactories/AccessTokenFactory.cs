using Alphaeon.Services.EnterpriseAPI.ApplicationServices.HashingServices;
using Alphaeon.Services.EnterpriseAPI.Models;

namespace Alphaeon.Services.EnterpriseAPI.DAL.ModelFactories
{
    sealed class AccessTokenFactory
    {
        private const int ValidHours = 12;

        public static AccessToken CreateNew(int applicationTokenId, int userTokenId)
        {
            var token = HashingService.GenerateToken();

            var accessToken = new AccessToken
            {
                Token = token
            };

            accessToken.EndDate = accessToken.StartDate.AddHours(ValidHours);

            return accessToken;
        }
    }
}
