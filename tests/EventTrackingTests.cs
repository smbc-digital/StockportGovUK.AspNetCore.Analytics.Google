using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Xunit;

namespace StockportGovUK.AspNetCore.Analytics.Google.Tests
{
    public class EventTrackingTests
    {
        private const string TrackingCode = "UA-XXXXXXXXX-X";
        private const string GatewayMethod = "SendAsync";
        private readonly Mock<HttpMessageHandler> _mockMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        private readonly EventTrackingHelper _eventTrackingHelper;

        public EventTrackingTests()
        {
            var mockConfigurationSection = new Mock<IConfigurationSection>();
            mockConfigurationSection
                .Setup(_ => _.Value)
                .Returns(TrackingCode);

            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration
                .Setup(_ => _.GetSection(It.IsAny<string>()))
                .Returns(mockConfigurationSection.Object);

            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    GatewayMethod,
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage { StatusCode = HttpStatusCode.OK })
                .Verifiable();
                
            var httpClient = new HttpClient(_mockMessageHandler.Object);

            mockHttpClientFactory
                .Setup(_ => _.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            _eventTrackingHelper = new EventTrackingHelper(mockConfiguration.Object, mockHttpClientFactory.Object);
        }

        [Fact]
        public void TrackEvent_IfNoCategoryPassed_ShouldThrowError()
        {
            Assert.Throws<ArgumentNullException>(() => _eventTrackingHelper.TrackEvent(null, "action"));
        }

        [Fact]
        public void TrackEvent_IfNoActionPassed_ShouldThrowError()
        {
            Assert.Throws<ArgumentNullException>(() => _eventTrackingHelper.TrackEvent("category", null));
        }

        [Fact]
        public void TrackEvent_IfArgumentsPassed_ShouldCallPostAsync()
        {
            _eventTrackingHelper.TrackEvent("category", "action");
            _mockMessageHandler
                .Protected()
                .Verify(
                    GatewayMethod,
                    Times.Exactly(1),
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post
                        && req.RequestUri == new Uri("https://www.google-analytics.com/collect")
                    ),
                    ItExpr.IsAny<CancellationToken>());
        }
    }
}
