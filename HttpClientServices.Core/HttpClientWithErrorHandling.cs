using HttpClientServices.Core.Exceptions;
using HttpClientServices.Core.Interfaces;
using HttpClientServices.Core.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpClientServices.Core
{
    public class HttpClientWithErrorHandling : IHttpClientWrapper
    {
        public IHttpClientWrapper Inner { get; }

        public HttpClientWithErrorHandling(IHttpClientWrapper inner)
        {
            Inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public async Task<HttpResponse<TSuccessResponse>> SendAsync<TSuccessResponse>(HttpRequestMessage message)
        {
            try
            {
                return await Inner.SendAsync<TSuccessResponse>(message);
            }
            catch (Exception ex)
            {
                return new HttpResponse<TSuccessResponse>
                {
                    Errors = new HttpAggregateException(ex)
                };
            }
        }
    }
}
