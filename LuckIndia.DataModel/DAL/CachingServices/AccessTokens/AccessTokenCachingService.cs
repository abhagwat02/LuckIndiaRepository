using LuckIndia.APIs.DAL.Exceptions;
using LuckIndia.DataModel;
using LuckIndia.DataModel.DAL.CachingServices;
using LuckIndia.DataModel.DAL.ValidationServices.ExpirableValidationServices;
using System.Collections.Concurrent;
using System.Data.Entity;
using System.Linq;

namespace Alphaeon.Services.EnterpriseAPI.DAL.CachingServices.AccessTokens
{
    public sealed class AccessTokenCachingService //: IRefreshable
    {
        private readonly ConcurrentDictionary<string, AccessTokenCacheModel> _cache = new ConcurrentDictionary<string, AccessTokenCacheModel>();

        private AccessTokenCachingService() { }

        public AccessTokenCacheModel GetAccessTokenCacheModel(string token)
        {
            if (_cache.ContainsKey(token))
            {
                return _cache[token];
            }

            this.RefreshCacheForToken(token);   

            return _cache[token];
        }

        private void RefreshCacheForToken(string token)
        {
            using (var context = LuckIndiaDBContext.GetContext())
            {
                var accessToken = context.AccessTokens
                    .Include(x => x.User)
                   // .Include(x => x.Application)
                    .FirstOrDefault(x => x.Token == token);

                if (accessToken == null ||
                    !ExpirableValidator.IsActive(accessToken.StartDate, accessToken.EndDate))
                {
                    throw new InvalidAccessTokenException();
                }

                var accessTokenCache = new AccessTokenCacheModel
                {
                    AccessToken = accessToken,
                    ApplicationId = accessToken.ApplicationId,
                    UserId = accessToken.UserId
                };

                _cache.TryAdd(token, accessTokenCache);
            }
        }

        public void RemoveFromCache(string token)
        {
            AccessTokenCacheModel cacheModel;
            _cache.TryRemove(token, out cacheModel);
        }

        public void Refresh()
        {
            //nothing to refresh since AccessToken are dynamically cached
        }

        private readonly static AccessTokenCachingService CachingService = new AccessTokenCachingService();
        public static AccessTokenCachingService GetCurrent()
        {
            return CachingService;
        }
    }
}
