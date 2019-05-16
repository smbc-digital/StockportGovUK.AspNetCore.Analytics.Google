namespace StockportGovUK.AspNetCore.Analytics.Google
{
    public interface IEventTrackingHelper
    {
        void TrackEvent(string category, string action);
        void TrackEvent(string category, string action, string label, int? value = null);
    }
}