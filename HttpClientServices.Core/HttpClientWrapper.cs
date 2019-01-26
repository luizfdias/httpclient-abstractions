using HttpClientServices.Core.Interfaces;
using HttpClientServices.Core.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace HttpClientServices.Core
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        public HttpClient HttpClient { get; }

        public ISerialization Serializer { get; }

        public Dictionary<HttpStatusCode, Type> UnsucessfulResponse { get; }

        public HttpClientWrapper(HttpClient httpClient, ISerialization serializer, Dictionary<HttpStatusCode, Type> unsucessfulResponse)
        {
            HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            Serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            UnsucessfulResponse = unsucessfulResponse;            
        }

        public async Task<HttpResponse<TSuccessResponse>> SendAsync<TSuccessResponse>(HttpRequestMessage message)
        {
            var result = new HttpResponse<TSuccessResponse>()
            {
                StatusCode = 0,
                Success = false
            };

            result.ResponseMessage = await HttpClient.SendAsync(message);
            result.OriginalBody = await result.ResponseMessage?.Content?.ReadAsStringAsync();
            result.Success = result.ResponseMessage.IsSuccessStatusCode;
            result.StatusCode = result.ResponseMessage.StatusCode;

            if (!string.IsNullOrWhiteSpace(result.OriginalBody))
            {
                if (result.ResponseMessage.IsSuccessStatusCode)
                {
                    result.Response = (TSuccessResponse)Serializer.Deserialize(result.OriginalBody, typeof(TSuccessResponse));
                }

                if (UnsucessfulResponse != null && UnsucessfulResponse.ContainsKey(result.ResponseMessage.StatusCode))
                {
                    result.UnsucessfulResponse = Serializer.Deserialize(result.OriginalBody, UnsucessfulResponse[result.ResponseMessage.StatusCode]);
                }
            }

            return result;
        }
    }
}