using System;
using System.Diagnostics.CodeAnalysis;
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
        private const string CustomerId = "smbc";
        private const string DataSource = "tests";
        private const string GatewayMethod = "SendAsync";
        private readonly Mock<HttpMessageHandler> _mockMessageHandler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        private readonly EventTrackingHelper _eventTrackingHelper;

        public EventTrackingTests()
        {
            var mockConfiguration = SetupConfiguration(false, false, false);
            _eventTrackingHelper = SetupEventTrackingHelper(mockConfiguration);
        }

        [Fact]
        public void TrackEvent_IfNoTrackingIdSetInConfig_ShouldThrowError()
        {
            var mockConfiguration = SetupConfiguration(true, false, false);
            Assert.Throws<ArgumentNullException>(() => SetupEventTrackingHelper(mockConfiguration));
        }

        [Fact]
        public void TrackEvent_IfNoCustomerIdSetInConfig_ShouldThrowError()
        {
            var mockConfiguration = SetupConfiguration(false, true, false);
            Assert.Throws<ArgumentNullException>(() => SetupEventTrackingHelper(mockConfiguration));
        }

        [Fact]
        public void TrackEvent_IfNoDataSourceSetInConfig_ShouldThrowError()
        {
            var mockConfiguration = SetupConfiguration(false, false, true);
            Assert.Throws<ArgumentNullException>(() => SetupEventTrackingHelper(mockConfiguration));
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

        [Fact]
        public void TrackEvent_IfArgumentsPassed_PostAsyncShouldContatinTrackingCode()
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
                        && req.Content.ReadAsStringAsync().Result.Contains(TrackingCode)
                    ),
                    ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public void TrackEvent_IfCategoryPassed_PostAsyncShouldContatinCategory()
        {
            const string Category = "category";
            _eventTrackingHelper.TrackEvent(Category, "action");
            _mockMessageHandler
                .Protected()
                .Verify(
                    GatewayMethod,
                    Times.Exactly(1),
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post
                        && req.RequestUri == new Uri("https://www.google-analytics.com/collect")
                        && req.Content.ReadAsStringAsync().Result.Contains(Category)
                    ),
                    ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public void TrackEvent_IfNoCategoryPassed_ShouldThrowError()
        {
            Assert.Throws<ArgumentNullException>(() => _eventTrackingHelper.TrackEvent(null, "action"));
        }

        [Fact]
        public void TrackEvent_IfActionPassed_PostAsyncShouldContatinAction()
        {
            const string Action = "action";
            _eventTrackingHelper.TrackEvent("category", Action);
            _mockMessageHandler
                .Protected()
                .Verify(
                    GatewayMethod,
                    Times.Exactly(1),
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post
                        && req.RequestUri == new Uri("https://www.google-analytics.com/collect")
                        && req.Content.ReadAsStringAsync().Result.Contains(Action)
                    ),
                    ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public void TrackEvent_IfNoActionPassed_ShouldThrowError()
        {
            Assert.Throws<ArgumentNullException>(() => _eventTrackingHelper.TrackEvent("category", null));
        }

        [Fact]
        public void TrackEvent_IfLabelPassed_PostAsyncShouldContatinLabel()
        {
            const string Label = "label";
            _eventTrackingHelper.TrackEvent("category", "action", Label);
            _mockMessageHandler
                .Protected()
                .Verify(
                    GatewayMethod,
                    Times.Exactly(1),
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post
                        && req.RequestUri == new Uri("https://www.google-analytics.com/collect")
                        && req.Content.ReadAsStringAsync().Result.Contains("&el")
                        && req.Content.ReadAsStringAsync().Result.Contains(Label)
                    ),
                    ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public void TrackEvent_IfNoLabelPassed_ShouldCallPostAsyncWithoutElValue()
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
                        && !req.Content.ReadAsStringAsync().Result.Contains("&el")
                    ),
                    ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public void TrackEvent_IfValuePassed_PostAsyncShouldContatinValue()
        {
            int Value = 1;
            _eventTrackingHelper.TrackEvent("category", "action", "label", Value);
            _mockMessageHandler
                .Protected()
                .Verify(
                    GatewayMethod,
                    Times.Exactly(1),
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post
                        && req.RequestUri == new Uri("https://www.google-analytics.com/collect")
                        && req.Content.ReadAsStringAsync().Result.Contains("&ev")
                        && req.Content.ReadAsStringAsync().Result.Contains(Value.ToString())
                    ),
                    ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public void TrackEvent_IfNoValuePassed_ShouldCallPostAsyncWithoutEvValue()
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
                        && !req.Content.ReadAsStringAsync().Result.Contains("&ev")
                    ),
                    ItExpr.IsAny<CancellationToken>());
        }

        private Mock<IConfiguration> SetupConfiguration(bool omitTrackingCode, bool omitCustomerId, bool omitDataSource)
        {
            var mockConfiguration = new Mock<IConfiguration>();

            var mockTrackingCodeSection = new Mock<IConfigurationSection>();
            mockTrackingCodeSection
                .Setup(_ => _.Value)
                .Returns(TrackingCode);

            var mockCustomerIdSection = new Mock<IConfigurationSection>();
            mockCustomerIdSection
                .Setup(_ => _.Value)
                .Returns(CustomerId);

            var mockDataSourceSection = new Mock<IConfigurationSection>();
            mockDataSourceSection
                .Setup(_ => _.Value)
                .Returns(DataSource);

            if (!omitTrackingCode)
            {
                mockConfiguration
                    .Setup(_ => _.GetSection("Analytics:TrackingCode"))
                    .Returns(mockTrackingCodeSection.Object);
            }

            if (!omitCustomerId)
            {
                mockConfiguration
                    .Setup(_ => _.GetSection("Analytics:CustomerId"))
                    .Returns(mockCustomerIdSection.Object);
            }

            if (!omitDataSource)
            {
                mockConfiguration
                    .Setup(_ => _.GetSection("Analytics:DataSource"))
                    .Returns(mockDataSourceSection.Object);
            }

            return mockConfiguration;
        }

        private EventTrackingHelper SetupEventTrackingHelper(Mock<IConfiguration> mockConfiguration)
        {
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

            return new EventTrackingHelper(mockConfiguration.Object, mockHttpClientFactory.Object);
        }
    }
}
