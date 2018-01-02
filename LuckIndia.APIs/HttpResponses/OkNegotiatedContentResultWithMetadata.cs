using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace LuckIndia.APIs.HttpResponses

{
    public class OkNegotiatedContentResultWithMetadata<T> : OkNegotiatedContentResult<T>
    {
        private readonly string _medatdata;

        public OkNegotiatedContentResultWithMetadata(T content, string metadata, ApiController controller)
            : base(content, controller)
        {
            this._medatdata = metadata;
        }

        public override Task<HttpResponseMessage> ExecuteAsync(CancellationToken cancellationToken)
        {
            var response = base.ExecuteAsync(cancellationToken).Result;
            response.Headers.Add("X-Metadata", this._medatdata);
            return Task.FromResult(response);
        }
    }
}
