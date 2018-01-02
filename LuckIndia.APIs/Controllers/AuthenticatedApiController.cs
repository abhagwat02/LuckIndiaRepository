using LuckIndia.DataModel;
using LuckIndia.DataModel.Authorizations;
using System.Web.Http;

namespace LuckIndia.APIs.Controllers

{
    //[AccessTokenAuthorize]
    public abstract class AuthenticatedApiController : BaseApiController
    {
        /// <summary>
        /// Returns a new DatabaseContext scoped with the current access token and logger.
        /// Typically do a using statement when using this method.
        /// </summary>
        [NonAction]
        public LuckIndiaDBContext GetDatabaseContext()
        {
            return LuckIndiaDBContext.GetContextWithAccessToken(this.GetAccessToken(), this.GetLogger());
        }
    }
}
