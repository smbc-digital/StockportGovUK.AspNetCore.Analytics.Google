using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StockportGovUK.AspNetCore.Analytics
{
    public static class GoogleAnalyticsHtmlExtension
    {
        public static IHtmlContent GoogleAnalytics<TModel>(this IHtmlHelper<TModel> html, string trackingCode)
        {
            var scriptBuilder = new TagBuilder("script");
            scriptBuilder.Attributes.Add("type", "text/javascript");
            scriptBuilder.InnerHtml.AppendHtml(@"(function (i, s, o, g, r, a, m) {
                i['GoogleAnalyticsObject'] = r; i[r] = i[r] || function () {
                    (i[r].q = i[r].q || []).push(arguments)
                }, i[r].l = 1 * new Date(); a = s.createElement(o),
                    m = s.getElementsByTagName(o)[0]; a.async = 1; a.src = g; m.parentNode.insertBefore(a, m)
                })(window, document, 'script', '//www.google-analytics.com/analytics.js', 'ga');");
            scriptBuilder.InnerHtml.AppendHtml($"ga('create', '{trackingCode}', 'auto');");
            scriptBuilder.InnerHtml.AppendHtml("ga('send', 'pageview');");
            return scriptBuilder;
        }
    }
}