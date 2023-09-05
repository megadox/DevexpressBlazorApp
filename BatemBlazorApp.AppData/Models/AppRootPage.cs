namespace BatemBlazorApp.AppData
{
    public class AppRootPage : AppPageBase
    {
        public string AnalyticsId { get; set; }

        public override AppItem[] GetChildItems() { return Pages; }
    }
}
