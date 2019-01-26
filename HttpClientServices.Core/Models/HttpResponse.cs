using HttpClientServices.Core.Exceptions;
using System.Net;
using System.Net.Http;

namespace HttpClientServices.Core.Models
{
    public class HttpResponse<T>
    {
        public bool Success { get; set; }

        public string OriginalBody { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public T Response { get; set; }

        public object UnsucessfulResponse { get; set; }

        public HttpAggregateException Errors { get; set; }

        public HttpResponseMessage ResponseMessage { get; set; }
    }
}
