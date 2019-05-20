using Microsoft.Extensions.DependencyInjection;

namespace StockportGovUK.AspNetCore.Analytics.Google
{
    public static class ServiceCollectionExtensions
    {
        public static void AddGoogleAnalytics(this IServiceCollection services)
        {
            services.AddSingleton<IEventTrackingHelper, EventTrackingHelper>();
        }
    }
}