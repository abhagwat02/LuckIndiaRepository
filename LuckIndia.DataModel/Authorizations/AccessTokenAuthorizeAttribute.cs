using LuckIndia.DataModel.ExceptionHttpResponseMessages;
using System.ServiceModel.Channels;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace LuckIndia.DataModel.Authorizations
{
    /// <summary>
    /// This class defines Access Token Authorize Attribute and overrides methods of AuthorizeAttribute
    /// </summary>
    public class AccessTokenAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        ///  This method return true if Authorized
        /// </summary>
        /// <param name="actionContext">Object of HttpActionContext</param>
        /// <returns></returns>
        protected override bool IsAuthorized(HttpActionContext actionContext)
        {
            var principal = actionContext.Request.GetUserPrincipal() as ApiPrincipal;
            return principal != null;
        }

        /// <summary>
        /// This method calls when an action is being authorized else sends response as unauthorized
        /// </summary>
        /// <param name="actionContext">Object of HttpActionContext</param>
        public override void OnAuthorization(HttpActionContext actionContext)
        {
            if (!this.IsAuthorized(actionContext))
            {
                var response = new UnauthorizedResponseMessage();
                actionContext.Response = response.GetHttpResponseMessage();
            }
            else
            {
                base.OnAuthorization(actionContext);
            }
        }
    }
}