using System;

namespace HttpClientServices.Core.Exceptions
{
    public class HttpAggregateException : AggregateException
    {
        public HttpAggregateException(params Exception[] innerExceptions) : base(innerExceptions)
        {
        }
    }
}
