using System;
using System.Diagnostics;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Alphaeon.Services.EnterpriseAPI.ApplicationServices.Authorizations;
using Alphaeon.Services.EnterpriseAPI.ApplicationServices.DataInjectors;
using Alphaeon.Services.EnterpriseAPI.ApplicationServices.LoggingServices;
using Alphaeon.Services.EnterpriseAPI.DAL;

namespace Alphaeon.Services.EnterpriseAPI.ApplicationServices.HttpHandlers
{
    public class ODataMyUserIdHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (request.Method != HttpMethod.Get)
            {
                return base.SendAsync(request, cancellationToken);
            }

            var principal = request.GetUserPrincipal() as ApiPrincipal;
            if (principal == null)
            {
                return base.SendAsync(request, cancellationToken);
            }

            var query = request.RequestUri.Query;

            if (!MyUserIdInjector.HasOccurences(query))
            {
                return base.SendAsync(request, cancellationToken);
            }

            var logger = HttpContext.Current.Items[WebApiConfig.LoggerHttpContextItemsKey] as ILogger;

            try
            {
                using (var context = CMDDatabaseContext.GetContextWithAccessToken(principal.AccessToken, logger))
                {
                    var userId = context.GetCurrentUser().Id;

                    var injector = new MyUserIdInjector(userId);

                    if (!injector.TryParseOccurences(query, out query))
                    {
                        return base.SendAsync(request, cancellationToken);
                    }
                }

                var uri = new UriBuilder(request.RequestUri) { Query = query.TrimStart(new[] { '?' }) };
                request.RequestUri = uri.Uri;
            }
            catch (Exception ex)
            {
                CMDApiLogger.LogException(ex, EventCategory.HttpHandlers, EventLogEntryType.Warning);
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
