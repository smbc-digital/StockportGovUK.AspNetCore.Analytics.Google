using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using Microsoft.Extensions.Configuration;

namespace StockportGovUK.AspNetCore.Analytics.Google
{
    public class EventTrackingHelper : IEventTrackingHelper
    {
        private readonly string _trackingCode;
        private readonly string _customerId;
        private readonly string _dataSource;
        private readonly HttpClient _client;

        public EventTrackingHelper(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            try
            {
                _trackingCode = configuration.GetValue<string>("Analytics:TrackingCode");
            }
            catch (Exception)
            {
                throw new ArgumentNullException("Tracking Code");
            }

            try
            {
                _customerId = configuration.GetValue<string>("Analytics:CustomerId");
            }
            catch (Exception)
            {
                throw new ArgumentNullException("Customer Id");
            }

            try
            {
                _dataSource = configuration.GetValue<string>("Analytics:DataSource");
            }
            catch (Exception)
            {
                throw new ArgumentNullException("Data Source");
            }

            _client = clientFactory.CreateClient();
        }

        public void TrackEvent(string category, string action)
        {
            TrackEvent(category, action, string.Empty);
        }

        public void TrackEvent(string category, string action, string label, int? value = null)
        {
            if (string.IsNullOrEmpty(category))
            {
                throw new ArgumentNullException("category");
            }
            
            if (string.IsNullOrEmpty(action))
            {
                throw new ArgumentNullException("action");
            }

            var postData = new Dictionary<string, string>
                {
                    { "v", "1" },
                    { "tid", _trackingCode },
                    { "cid", _customerId },
                    { "ds", _dataSource },
                    { "t", "event" },
                    { "ec", category },
                    { "ea", action }
                };

            if (!string.IsNullOrEmpty(label))
            {
                postData.Add("el", label);
            }
            
            if (value.HasValue)
            {
                postData.Add("ev", value.ToString());
            }

            var content = new StringContent(postData
                .Aggregate("", (data, next) => $"{data}&{next.Key}={HttpUtility.UrlEncode(next.Value)}")
                .TrimEnd('&')
            );

            _client.PostAsync("https://www.google-analytics.com/collect", content);
        }
    }
}
