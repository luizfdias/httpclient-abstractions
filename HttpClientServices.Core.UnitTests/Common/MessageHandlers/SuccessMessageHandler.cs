using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace HttpClientServices.Core.UnitTests.Common.MessageHandlers
{
    public class SuccessMessageHandler : HttpMessageHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK);

            response.RequestMessage = request;
            response.Content = new StringContent("{\"field1\":\"value1\"}");

            return response;
        }
    }
}
