using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace LuckIndia.APIs.HttpResponses
{
    public sealed class OkAcceptedResult : OkResult
    {
        public OkAcceptedResult(ApiController controller) : base(controller) { }

        public override Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = base.ExecuteAsync(cancellationToken).Result;
            response.StatusCode = HttpStatusCode.Accepted;
            return Task.FromResult(response);
        }
    }
}