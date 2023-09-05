using DevExpress.XtraPrinting.Native;
using Microsoft.AspNetCore.Components;
using DevExpress.Blazor.DocumentMetadata;
using BatemBlazorApp.AppData;

namespace BatemBlazorApp.Configuration
{
    public class AppConfiguration
    {
        public const string DocBaseUrl = "https://docs.devexpress.com/Blazor/";
        public static readonly string PagesFolderName = "Pages";
        public static readonly string DescriptionsFolderName = "Descriptions";
        private IConfiguration Configuration { get; set; }
        public AppModel Model { get; private set; }
        public AppSearchHelper Search { get; private set; }

        public AppConfiguration() { }
        public AppConfiguration(IConfiguration configuration)
        {
            Configuration = configuration;

            Model = AppModel.Create(IsServerSide);
            Products = Model.Products;
            RootPages = Model.RootPages;
            Redirects = Model.Redirects ?? new Dictionary<string, string>();
            Search = new AppSearchHelper(Model.Search, RootPages);
        }

        public bool IsServerSide =>
#if SERVER_BLAZOR
            true;
#else
            false;
#endif
        public virtual bool ShowOnlyReporting => false;

        public virtual IEnumerable<AppProductInfo> Products { get; }
        public virtual IEnumerable<AppRootPage> RootPages { get; }
        public Dictionary<string, string> Redirects { get; private set; }

        public T GetConfigurationValue<T>(string key)
        {
            return Configuration.GetValue<T>(key);
        }

        public AppPageBase GetDemoPageByUrl(NavigationManager navigationManager, string currentUrl)
        {
            var demoPageUrl = navigationManager.ToAbsoluteUri(currentUrl).GetLeftPart(UriPartial.Path).Replace(navigationManager.BaseUri, "");
            return Model.GetDemoPageByUrl(demoPageUrl);
        }
        public AppItem GetAppItem(string id)
        {
            return Model.GetDemoItem(id);
        }

        string GetAppItemDescriptionResourcePath(AppItem item)
        {
            return Model.GetDemoItemDescriptionResourcePath(item, PagesFolderName);
        }
        public string GetDemoDescription(AppItem item)
        {
            string path = GetAppItemDescriptionResourcePath(item);
            return GetDemoFileContent(path);
        }
        string GetAppItemRazorResourcePath(AppItem item)
        {
            return Model.GetDemoItemRazorResourcePath(item, PagesFolderName);
        }
        public Dictionary<string, string> GetDemoCodeFiles(AppItem item)
        {
            var result = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
            if (item.IsRazorFileVisible())
            {
                string razorPath = GetAppItemRazorResourcePath(item);
                result.Add("Razor", GetDemoFileContent(razorPath));
            }
            foreach (var codeFile in item.GetAdditionalCodeFiles())
            {
                string codeFilePath = codeFile.Path.Replace("\\", ".");
                string codeFileContent = GetDemoFileContent(codeFilePath);
                result[codeFile.Title] = codeFile.GetPreparedContent(codeFileContent);
            }
            return result;
        }
        protected string GetDemoFileContent(string path)
        {
            return AppUtils.GetFileContent(typeof(AppConfiguration), "BlazorDemo." + path);
        }

        // Metadata
        public virtual void ConfigureMetadata(IDocumentMetadataCollection metadataCollection)
        {
            if (Model == null)
                return;

            metadataCollection.AddDefault()
                .Base("~/")
                .Charset("utf-8")
                .Viewport("width=device-width, initial-scale=1.0");

            var titleFormat = Model.TitleFormat ?? "{0}";
            foreach (var rootPage in Model.RootPages)
            {
                var title = rootPage.SeoTitle ?? rootPage.Title;
                ConfigurePage(metadataCollection, rootPage, title, titleFormat);
            }
        }
        static void ConfigurePage(IDocumentMetadataCollection metadataCollection, AppPageBase page, string title, string titleFormat, bool stopIndexation = false)
        {
            if (page.Url != null && !page.IsMaintenanceMode)
            {
                var pageUrl = page.Url == "./" ? "" : page.Url;
                var metaBuilder = metadataCollection.AddPage(pageUrl)
                    .OpenGraph("url", page.OG_Url)
                    .OpenGraph("type", page.OG_Type)
                    .OpenGraph("title", page.OG_Title)
                    .OpenGraph("description", page.OG_Description)
                    .OpenGraph("image", page.OG_Image)
                    .Title(string.Format(titleFormat, title))
                    .Meta("description", page.GetDescription())
                    .Meta("keywords", page.GetKeywords());

                if (stopIndexation)
                    metaBuilder.Meta("robots", "none");
            }
            foreach (var subPage in page.Pages)
                ConfigurePage(metadataCollection, subPage, string.Join(" - ", title, subPage.Title), titleFormat, page.IsMaintenanceMode);
        }
        // Search
        public List<AppSearchResult> DoSearch(string request)
        {
            return Search.DoSearch(request);
        }
    }
}
