using FluentAssertions;
using HttpClientServices.Core.Interfaces;
using HttpClientServices.Core.UnitTests.AutoData;
using HttpClientServices.Core.UnitTests.Common;
using HttpClientServices.Core.UnitTests.Common.MessageHandlers;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using Xunit;

namespace HttpClientServices.Core.UnitTests
{
    public class HttpClientWrapperTests
    {
        [Fact]
        public void Constructor_WhenMissingHttpClientDependency_ShouldThrowException()
        {
            var result =
                Assert.Throws<ArgumentNullException>(() =>
                new HttpClientWrapper(null, Substitute.For<ISerialization>(), new Dictionary<HttpStatusCode, Type>()));

            result.Message.Should().Contain("httpClient");
        }

        [Fact]
        public void Constructor_WhenMissingSerialization_ShouldThrowException()
        {
            var result =
                Assert.Throws<ArgumentNullException>(() =>
                new HttpClientWrapper(Substitute.For<HttpClient>(), null, new Dictionary<HttpStatusCode, Type>()));

            result.Message.Should().Contain("serializer");
        }

        [Fact]
        public void Constructor_WhenAllDependenciesAreProvided_ShouldConstructInstanceAsExpected()
        {
            var result = new HttpClientWrapper(Substitute.For<HttpClient>(), Substitute.For<ISerialization>(), null);

            result.Should().NotBeNull();
        }

        [Theory, AutoNSubstituteData]
        public async void SendAsync_WhenResponseIsSuccessAndHasData_ShouldReturnIt(HttpRequestMessage message)
        {
            var sut = new HttpClientWrapper(new HttpClient(new SuccessMessageHandler()), Substitute.For<ISerialization>(), null);

            sut.Serializer.Deserialize(Arg.Any<string>(), typeof(Foo)).Returns(new Foo
            {
                Field1 = "value1"
            });

            var result = await sut.SendAsync<Foo>(message);

            result.Success.Should().BeTrue();
            result.Response.Field1.Should().Be("value1");
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.OriginalBody.Should().Be("{\"field1\":\"value1\"}");
        }

        [Theory, AutoNSubstituteData]
        public async void SendAsync_WhenHttpMethodIsGETAndResponseIsSuccess_ResultShouldBeAsExpected(HttpRequestMessage message)
        {
            var sut = new HttpClientWrapper(new HttpClient(new SuccessMessageHandler()), Substitute.For<ISerialization>(), null);

            sut.Serializer.Deserialize(Arg.Any<string>(), typeof(Foo)).Returns(new Foo
            {
                Field1 = "value1"
            });

            message.Content = null;
            message.Method = HttpMethod.Get;

            var result = await sut.SendAsync<Foo>(message);

            result.Success.Should().BeTrue();
            result.Response.Field1.Should().Be("value1");
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.OriginalBody.Should().Be("{\"field1\":\"value1\"}");
        }

        [Theory, AutoNSubstituteData]
        public async void SendAsync_WhenResponseIsSuccessButContentIsEmpty_ResultShouldBeAsExpected(HttpRequestMessage message)
        {
            var sut = new HttpClientWrapper(new HttpClient(new SuccessEmptyMessageHandler()), Substitute.For<ISerialization>(), null);

            var result = await sut.SendAsync<Foo>(message);

            result.Success.Should().BeTrue();
            result.Response.Should().BeNull();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.OriginalBody.Should().BeNullOrEmpty();
            sut.Serializer.DidNotReceive().Deserialize(Arg.Any<string>(), typeof(Foo));
        }

        [Theory, AutoNSubstituteData]
        public async void SendAsync_WhenUnsuccessfulResponse_ResultShouldBeAsExpected(HttpRequestMessage message)
        {
            var sut = new HttpClientWrapper(new HttpClient(new FailedMessageHandler()), Substitute.For<ISerialization>(),
                new Dictionary<HttpStatusCode, Type> { { HttpStatusCode.BadRequest, typeof(Foo) } });

            sut.Serializer.Deserialize(Arg.Any<string>(), typeof(Foo)).Returns(new Foo
            {
                Field1 = "value1"
            });

            var result = await sut.SendAsync<Foo>(message);

            result.Success.Should().BeFalse();
            result.Response.Should().BeNull();
            result.UnsucessfulResponse.Should().BeOfType<Foo>();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.OriginalBody.Should().Be("{\"field1\":\"value1\"}");
        }

        [Theory, AutoNSubstituteData]
        public async void SendAsync_WhenUnsuccessfulResponseIsNotFound_ResultShouldBeAsExpected(HttpRequestMessage message)
        {
            var sut = new HttpClientWrapper(new HttpClient(new FailedMessageHandler()), Substitute.For<ISerialization>(),
                new Dictionary<HttpStatusCode, Type> { { HttpStatusCode.InternalServerError, typeof(Foo) } });

            var result = await sut.SendAsync<Foo>(message);

            result.Success.Should().BeFalse();
            result.Response.Should().BeNull();
            result.UnsucessfulResponse.Should().BeNull();
            result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            result.OriginalBody.Should().Be("{\"field1\":\"value1\"}");
            sut.Serializer.DidNotReceive().Deserialize(Arg.Any<string>(), typeof(Foo));
        }
    }
}
