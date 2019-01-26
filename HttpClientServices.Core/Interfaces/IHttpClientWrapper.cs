using HttpClientServices.Core.Models;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpClientServices.Core.Interfaces
{
    public interface IHttpClientWrapper
    {
        Task<HttpResponse<TSuccessResponse>> SendAsync<TSuccessResponse>(HttpRequestMessage message);
    }
}
