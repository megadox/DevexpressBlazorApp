using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BatemBlazorApp.AppData
{
    public abstract class AppItem
    {
        public string[] RedirectFrom { get; set; }
        public string Id { get; set; }
        public string Title { get; set; }
        public string TitleOnPage { get; set; }
        public bool IsServerSideOnly { get; set; }
        public bool IsClientSideOnly { get; set; }
        public bool IsNew { get; set; }
        public bool IsUpdated { get; set; }
        public string DocUrl { get; set; }
        public AppCodeFile[] AdditionalCodeFiles { get; set; }
        public bool? ShowRazorFile { get; set; }

        [JsonIgnore]
        public AppPageBase ParentPage { get; set; }
        [JsonIgnore]
        public AppRootPage RootPage { get; set; }
        [JsonIgnore]
        public string UniqueId { get { return string.Join("-", GetUniqueIdParts()); } }
        [JsonIgnore]
        public string UniqueResourceId { get { return string.Join(".", GetUniqueIdParts()); } }

        public abstract string GetRazorFilesFolder();
        public abstract string GetDescriptionFilesFolder();
        public abstract string GetUrl();
        public abstract AppItem[] GetChildItems();

        public AppCodeFile[] GetAdditionalCodeFiles()
        {
            if (AdditionalCodeFiles != null && AdditionalCodeFiles.Any())
                return AdditionalCodeFiles;
            if (ParentPage != null)
                return ParentPage.GetAdditionalCodeFiles();
            return Array.Empty<AppCodeFile>();
        }
        public bool IsRazorFileVisible()
        {
            if (ShowRazorFile.HasValue && !ShowRazorFile.Value)
                return false;
            if (ParentPage != null)
                return ParentPage.IsRazorFileVisible();
            return true;
        }

        public string[] GetUniqueIdParts()
        {
            List<string> result = new List<string>();
            var item = this;
            while (item != null)
            {
                result.Insert(0, item.Id);
                item = item.ParentPage;
            }
            return result.ToArray();
        }

        public AppItemStatus GetStatus()
        {
            var parentPage = ParentPage;
            if (parentPage != null)
            {
                var siblingItems = parentPage.GetChildItems();
                if (siblingItems.All(p => p.IsNew))
                    return AppItemStatus.Empty;

                while (parentPage != null)
                {
                    if (parentPage.IsNew || parentPage.IsMaintenanceMode)
                        return AppItemStatus.Empty;
                    parentPage = parentPage.ParentPage;
                }
            }
            return GetStatusCore();
        }
        protected virtual AppItemStatus GetStatusCore()
        {
            if (IsNew)
                return AppItemStatus.New;
            var childItems = GetChildItems();
            if (childItems.Any() && childItems.All(p => p.GetStatusCore() == AppItemStatus.New))
                return AppItemStatus.New;
            if (childItems.Any(p => p.GetStatusCore() == AppItemStatus.New || p.GetStatusCore() == AppItemStatus.Updated))
                return AppItemStatus.Updated;
            if (IsUpdated)
                return AppItemStatus.Updated;
            return AppItemStatus.Empty;
        }

        public string GetStatusMessageMarkdown()
        {
            var page = FindPage(page => page.IsPreview);
            if (page != null)
            {
                if (!string.IsNullOrEmpty(page.PreviewMessage))
                    return page.PreviewMessage;
                return string.Format("The {0} is currently available as a community technology preview [(CTP)](https://www.devexpress.com/aboutus/pre-release.xml).", page.Title);
            }
            page = FindPage(page => page.IsMaintenanceMode);
            if (page != null)
            {
                if (!string.IsNullOrEmpty(page.MaintenanceModeMessage))
                    return page.MaintenanceModeMessage;
                return string.Format("The {0} was moved to maintenance support mode. No new features/capabilities will be added to this component.", page.Title);
            }
            return string.Empty;
        }
        protected virtual AppPageBase FindPage(Func<AppPageBase, bool> findFunc)
        {
            var page = ParentPage;
            while (page != null)
            {
                if (findFunc(page))
                    return page;
                page = page.ParentPage;
            }
            return null;
        }
    }

    public enum AppItemStatus { Empty, Preview, MaintenanceMode, New, Updated }
}
