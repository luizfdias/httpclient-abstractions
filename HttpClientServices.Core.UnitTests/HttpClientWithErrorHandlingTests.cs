using AutoFixture.Idioms;
using FluentAssertions;
using HttpClientServices.Core.Exceptions;
using HttpClientServices.Core.Models;
using HttpClientServices.Core.UnitTests.AutoData;
using HttpClientServices.Core.UnitTests.Common;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace HttpClientServices.Core.UnitTests
{
    public class HttpClientWithErrorHandlingTests
    {
        [Theory, AutoNSubstituteData]
        public void Constructor_WhenMissingDependencies_ShouldThrowException(GuardClauseAssertion guardClauseAssertion)
        {
            guardClauseAssertion.Verify(typeof(HttpClientWithErrorHandling).GetConstructors());
        }

        [Theory, AutoNSubstituteData]
        public async void SendAsync_WhenSendAsyncIsSuccess_ErrorsShouldBeNull(HttpClientWithErrorHandling sut, HttpRequestMessage message,
            Task<HttpResponse<Foo>> responseWrapper)
        {
            (await responseWrapper).Errors = null;

            sut.Inner.SendAsync<Foo>(message).Returns(responseWrapper);

            var result = await sut.SendAsync<Foo>(message);

            result.Errors.Should().BeNull();
        }

        [Theory, AutoNSubstituteData]
        public async void SendAsync_WhenSendAsyncThrowsException_ResultShouldBeFailed(HttpClientWithErrorHandling sut, HttpRequestMessage message)
        {
            sut.Inner.SendAsync<Foo>(message).Throws((new TimeoutException()));

            var result = await sut.SendAsync<Foo>(message);

            result.Errors.Should().BeOfType<HttpAggregateException>();
            result.Errors.InnerException.Should().BeOfType<TimeoutException>();
        }
    }
}
