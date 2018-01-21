using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Alphaeon.Services.EnterpriseAPI.ApplicationServices.DataInjectors;

namespace Alphaeon.Services.EnterpriseAPI.ApplicationServices.HttpHandlers
{
    public class ODataUtcNowHandler : DelegatingHandler
    {
        private readonly UtcNowInjector _injector = new UtcNowInjector();

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var query = request.RequestUri.Query;

                if (request.Method == HttpMethod.Get && _injector.TryParseOccurences(query, out query))
                {
                    var uri = new UriBuilder(request.RequestUri) {Query = query.TrimStart(new[] {'?'})};
                    request.RequestUri = uri.Uri;
                }
            }
            catch
            {
                //fail silently and let the normal flow happen
            }

            return base.SendAsync(request, cancellationToken);
        }
    }
}
