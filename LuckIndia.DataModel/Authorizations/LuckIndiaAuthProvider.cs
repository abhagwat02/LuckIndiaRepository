using System;
using System.Linq;
using Alphaeon.Services.EnterpriseAPI.DAL.CachingServices.AccessTokens;
using LuckIndia.DataModel.Enums;
using System.Web;
using LuckIndia.DataModel.Interfaces;

namespace LuckIndia.DataModel.Authorizations.Authorizations
{
    public class LuckIndiaAuthProvider : IAuthProvider
    {
        public AuthEnums.ProviderType AuthProviderType
        {
            get
            {
                return AuthEnums.ProviderType.LuckIndia;
            }
        }

        public string GetToken(HttpContext context)
        {
            throw new NotImplementedException();
        }

        SecurityContext IAuthProvider.Verify(string token)
        {
           // AuthorizedAccessToken authAcessToken = new AuthorizedAccessToken();
           SecurityContext securityContext = new SecurityContext();
            using (LuckIndiaDBContext context = new LuckIndiaDBContext())
            {

                var cacheModel =  AccessTokenCachingService.GetCurrent().GetAccessTokenCacheModel(token);
                securityContext.AccessToken = cacheModel.AccessToken.Token;
                //securityContext.ApplicationInfo = new ApplicationInfo { Id = cacheModel.AccessToken.ApplicationId, Title = cacheModel.AccessToken.Application.Title };
                //CMD tokens wouldn't have expireation date.
                securityContext.EndDate = DateTime.UtcNow;
                securityContext.StartDate = DateTime.UtcNow;

                securityContext.User = new User
                {
                    Email = cacheModel.AccessToken.User.Email,
                    FirstName = cacheModel.AccessToken.User.FirstName,
                    Id = cacheModel.AccessToken.User.Id,
                    LastName = cacheModel.AccessToken.User.LastName,
                   

                };

                //var roles = context.UserRoles.Where(z => z.UserId == securityContext.User.Id).Select(z => z.Role);//cacheModel.AccessToken.UserToken.User.UserRoles;
                //foreach (Models.Role userrole in roles)
                //{
                //    securityContext.User.Roles.Add(
                //        new ALPHAEON.API.Common.SecurityHelpers.Role { Id = userrole.Id, Title = userrole.Title }
                //        );
                //}

            }
            return  securityContext;
            
         
        }
    }
}