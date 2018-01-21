using System.Net.Http;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using LusckIndia.APIs.Handlers;
using LuckIndia.DataModel.Authorizations;
using System;

namespace LuckIndia.APIs.Handlers
{ 
    public class SetApiPrincipalHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var httpContext = HttpContext.Current;
            var helper = new HttpRequestHelper(request);
            var token = helper.GetAccessToken();

            // AuthProviderFactory.GetCurrent().GetAuthProvider().GetToken(request);
            try
            {
                if (!string.IsNullOrWhiteSpace(token))
                {
                    HttpStatusCode status;
                    var principal = new ApiPrincipal(token, AuthProviderFactory.GetAuthProvider().Verify(token));//AccessTokenAuthorization.getAuthorizedAccessToken(token, out status));
                                                                                                                              //set this so other handlers can get the principal
                    request.SetUserPrincipal(principal);

                    //set these so other areas of the pipeline can get the pincipal
                    httpContext.User = principal;
                    Thread.CurrentPrincipal = principal;
                    return base.SendAsync(request, cancellationToken);

                }
            }
            catch (Exception ex)
            {
                request.CreateResponse(HttpStatusCode.Unauthorized);

            }
            return base.SendAsync(request, cancellationToken);

        }
    }


}
