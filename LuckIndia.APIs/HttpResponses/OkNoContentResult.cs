using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace LuckIndia.APIs.HttpResponses

{
    public class OkNoContentResult : OkResult
    {
        public OkNoContentResult(ApiController controller) : base(controller) { }

        public override Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = base.ExecuteAsync(cancellationToken).Result;
            response.StatusCode = HttpStatusCode.NoContent;
            return Task.FromResult(response);
        }
    }
}
