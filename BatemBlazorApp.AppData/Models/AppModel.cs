using Newtonsoft.Json;

namespace BatemBlazorApp.AppData
{
    public class AppModel
    {
        public string AssemblyName { get; set; }
        public string TitleFormat { get; set; }

        public AppRootPage[] RootPages { get; set; }
        public AppProductInfo[] Products { get; set; }
        public AppSearchModel Search { get; set; }

        [JsonIgnore]
        public Dictionary<string, string> Redirects { get; private set; }

        [JsonIgnore]
        public bool IsBlazorServer { get; private set; }

        public static AppModel Create(bool isBlazorServer)
        {
            var jsonContent = AppUtils.GetFileContent(typeof(AppModel), "BatemBlazorApp.AppData.demo-metadata.json");
            return Create(isBlazorServer, jsonContent);
        }
        public static AppModel Create(bool isBlazorServer, string jsonContent)
        {
            var model = JsonConvert.DeserializeObject<AppModel>(jsonContent);
            model.IsBlazorServer = isBlazorServer;
            model.Prepare();
            return model;
        }
        void Prepare()
        {
            Products = PrepareList(Products);
            RootPages = PrepareList(RootPages);
            Redirects = new Dictionary<string, string>();
            PrepareRecursive(RootPages, null, null);
        }
        void PrepareRecursive(IEnumerable<AppItem> childItems, AppPageBase parent, AppRootPage root)
        {
            foreach (var item in childItems)
            {
                if (item is AppRootPage rootPage)
                {
                    rootPage.Pages = PrepareList(rootPage.Pages);
                    PrepareRecursive(rootPage.Pages, rootPage, rootPage);
                }
                if (item is AppPage page)
                {
                    page.Pages = PrepareList(page.Pages);
                    page.PageSections = PrepareList(page.PageSections);
                    PrepareRecursive(page.GetChildItems(), page, root);
                }
                item.ParentPage = parent;
                item.RootPage = root;

                if (item.RedirectFrom?.Length > 0)
                {
                    foreach (var redirect in item.RedirectFrom)
                        Redirects.Add(redirect.ToLower(), item.GetUrl());
                }
            }
        }
        T[] PrepareList<T>(T[] list)
        {
            if (list == null)
                return Array.Empty<T>();
            IEnumerable<T> result = list;
            result = result
               .Where(i => i switch {
                   AppProductInfo info => !(IsBlazorServer ? info.IsClientSideOnly : info.IsServerSideOnly),
                   AppItem item => !(IsBlazorServer ? item.IsClientSideOnly : item.IsServerSideOnly),
                   _ => throw new NotSupportedException()
               });
            return result
                .OrderBy(i => i is AppPageBase page ? page.IsMaintenanceMode : false)
                .ToArray();
        }

        public AppPageBase GetDemoPageByUrl(string pageUrl)
        {
            pageUrl = pageUrl.Trim('/').Split('#')[0];

            AppPageBase FindRecursive(IEnumerable<AppPageBase> pages)
            {
                if (pages == null)
                    return null;
                foreach (var page in pages)
                {
                    if (string.Equals(page.Url, pageUrl, StringComparison.OrdinalIgnoreCase))
                        return page;
                    var nestedResult = FindRecursive(page.Pages);
                    if (nestedResult != null)
                        return nestedResult;
                }
                return null;
            }

            return FindRecursive(RootPages);
        }
        public AppItem GetDemoItem(string id)
        {
            string[] idParts = id.Split("-");
            AppItem result = null;
            for (var i = 0; i < idParts.Length; i++)
            {
                AppItem[] childItems = result != null ? result.GetChildItems() : RootPages;
                result = childItems.FirstOrDefault(p => p.Id == idParts[i]);
                if (result == null)
                    return null;
            }
            return result;
        }
        public string GetDemoItemDescriptionResourcePath(AppItem item, string rootFolder)
        {
            return GetDemoItemResourcePath(item, rootFolder, s => {
                var folder = item.GetDescriptionFilesFolder();
                return !string.IsNullOrEmpty(folder) ? folder : s + ".Descriptions";
            }, ".md");
        }
        public string GetDemoItemRazorResourcePath(AppItem item, string rootFolder)
        {
            return GetDemoItemResourcePath(item, rootFolder, s => {
                var folder = item.GetRazorFilesFolder();
                return !string.IsNullOrEmpty(folder) ? folder : s;
            }, ".razor");
        }
        string GetDemoItemResourcePath(AppItem item, string rootFolder, Func<string, string> getFolder, string extension)
        {
            string[] itemIds = item.GetUniqueIdParts();
            var partCount = itemIds.Length;
            if (item is AppPageSection || (!item.GetChildItems().Any() && item.ParentPage != item.RootPage))
                partCount--;
            List<string> pathParts = new List<string>();
            if (!string.IsNullOrEmpty(rootFolder))
                pathParts.Add(rootFolder);
            pathParts.Add(getFolder(string.Join(".", itemIds.Take(partCount))));
            pathParts.Add(string.Join("-", itemIds) + extension);
            return string.Join(".", pathParts);
        }
    }
}
