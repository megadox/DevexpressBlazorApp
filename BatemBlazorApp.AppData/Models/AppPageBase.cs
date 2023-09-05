namespace BatemBlazorApp.AppData
{
    public abstract class AppPageBase : AppItem
    {
        public AppPage[] Pages { get; set; }

        public string Url { get; set; }
        public string SeoTitle { get; set; }
        public string Keywords { get; set; }
        public string SearchKeywords { get; set; }
        public string Description { get; set; }

        public bool IsPreview { get; set; }
        public string PreviewMessage { get; set; }
        public bool IsMaintenanceMode { get; set; }
        public string MaintenanceModeMessage { get; set; }

        public string OG_Title { get; set; }
        public string OG_Image { get; set; }
        public string OG_Description { get; set; }
        public string OG_Url { get; set; }
        public string OG_Type { get; set; }

        public bool? ReCreateOnThemeChange { get; set; }

        public string RazorFilesFolder { get; set; }
        public string DescriptionFilesFolder { get; set; }

        public string GetDescription()
        {
            return !string.IsNullOrEmpty(Description) ? Description : ParentPage?.GetDescription();
        }
        public string GetKeywords()
        {
            return !string.IsNullOrEmpty(Keywords) ? Keywords : ParentPage?.GetKeywords();
        }

        public override string GetUrl()
        {
            return !string.IsNullOrEmpty(Url) ? Url : Pages.Select(p => p.GetUrl()).FirstOrDefault();
        }
        public override string GetRazorFilesFolder()
        {
            if (RazorFilesFolder == null)
                return ParentPage?.GetRazorFilesFolder();
            return UniqueResourceId + (string.IsNullOrWhiteSpace(RazorFilesFolder) ? "" : "." + RazorFilesFolder);
        }
        public override string GetDescriptionFilesFolder()
        {
            if (DescriptionFilesFolder == null)
                return ParentPage?.GetDescriptionFilesFolder();
            return UniqueResourceId + (string.IsNullOrWhiteSpace(DescriptionFilesFolder) ? "" : "." + DescriptionFilesFolder);
        }
        protected override AppItemStatus GetStatusCore()
        {
            if (IsPreview)
                return AppItemStatus.Preview;
            if (IsMaintenanceMode)
                return AppItemStatus.MaintenanceMode;
            return base.GetStatusCore();
        }
        protected override AppPageBase FindPage(Func<AppPageBase, bool> findFunc)
        {
            if (findFunc(this))
                return this;
            return base.FindPage(findFunc);
        }
    }
}
