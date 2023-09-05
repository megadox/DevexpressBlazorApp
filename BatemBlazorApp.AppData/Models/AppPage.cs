namespace BatemBlazorApp.AppData
{
    public class AppPage : AppPageBase
    {
        public AppPageSection[] PageSections { get; set; }

        public override AppItem[] GetChildItems() { return (PageSections.Length > 0) ? PageSections : Pages; }
    }
}
