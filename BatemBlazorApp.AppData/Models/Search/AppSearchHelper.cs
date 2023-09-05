namespace BatemBlazorApp.AppData
{
    public class AppSearchHelper
    {
        public AppSearchHelper(AppSearchModel searchModel, IEnumerable<AppRootPage> rootPages)
        {
            RootPages = rootPages;
            SearchAgregator = new AppSearchAgregator(searchModel);
        }

        public IEnumerable<AppRootPage> RootPages { get; private set; }
        public AppSearchAgregator SearchAgregator { get; private set; }

        protected Dictionary<AppItem, Dictionary<string, int>> KeywordsRankList { get; private set; } = new Dictionary<AppItem, Dictionary<string, int>>();

        public List<AppSearchResult> DoSearch(string request)
        {
            var results = new List<AppSearchResult>();
            if (!string.IsNullOrEmpty(request))
            {
                var requests = SearchAgregator.SplitRequests(request);
                try
                {
                    foreach (var rootPage in RootPages)
                    {
                        if (rootPage.IsMaintenanceMode) continue;

                        results.AddRange(DoSearch(requests, rootPage));
                    }
                }
                catch
                {
                }
                results = results.OrderByDescending(sr => sr.Rank).ToList();
            }
            return results;
        }
        IEnumerable<AppSearchResult> DoSearch(List<string[]> requests, AppPageBase page)
        {
            var results = new List<AppSearchResult>();
            foreach (var childPage in page.Pages)
            {
                if (childPage.IsMaintenanceMode) continue;

                if (childPage.Pages.Length > 0)
                    results.AddRange(DoSearch(requests, childPage));
                else
                {
                    int resultCount = results.Count;
                    foreach (var section in childPage.PageSections)
                        results.AddRange(GetRes(requests, section, AppSearchAgregator.HighlightOccurences(section.Title, requests)));
                    if (results.Count == resultCount)
                        results.AddRange(GetRes(requests, childPage, AppSearchAgregator.HighlightOccurences(childPage.Title, requests)));
                }
            }
            results.AddRange(GetRes(requests, page, AppSearchAgregator.HighlightOccurences(page.Title, requests)));
            return results;
        }
        IEnumerable<AppSearchResult> GetRes(List<string[]> requests, AppItem item, string text)
        {
            var results = new List<AppSearchResult>();
            var rank = CalculateRank(requests, item);
            if (rank > -1)
            {
                var sr = new AppSearchResult(item, rank);
                sr.Text = text;
                results.Add(sr);
            }
            return results;
        }
        int CalculateRank(List<string[]> requests, AppItem item)
        {
            int resultRank = 0;
            int keywordRank = 0;
            foreach (var request in requests)
            {
                int requestRank = -1;
                if (item is AppPageSection && item.ParentPage.GetChildItems().Length > 1 && AppSearchAgregator.CalculateRank(request, GetKeywordsRankList(item), out keywordRank))
                    requestRank += keywordRank;
                if (item is AppPageBase && AppSearchAgregator.CalculateRank(request, GetKeywordsRankList(item), out keywordRank))
                    requestRank += keywordRank;
                if (item is AppRootPage && AppSearchAgregator.CalculateRank(request, GetKeywordsRankList(item), out keywordRank))
                    requestRank += keywordRank;
                if (requestRank == -1 && SearchAgregator.WordsExclusions.Any(re => re.Equals(request[0], AppSearchAgregator.DefaultStringComparison)))
                    requestRank = 0;

                if (requestRank > -1)
                    resultRank += requestRank;
                else
                    return -1;
            }
            return resultRank;
        }
        Dictionary<string, int> GetKeywordsRankList(AppItem item)
        {
            if (!KeywordsRankList.ContainsKey(item))
            {
                List<TextRank> textRanks = new List<TextRank>();

                var rootPage = item as AppRootPage;
                var page = item as AppPage;

                if (rootPage != null)
                {
                    textRanks.Add(new TextRank(rootPage.Title, 15));
                    textRanks.Add(new TextRank(rootPage.Id, 7));
                    textRanks.Add(new TextRank(rootPage.SearchKeywords, 7));
                    textRanks.Add(new TextRank(rootPage.SeoTitle, 5));
                    textRanks.Add(new TextRank(rootPage.Keywords, 3));
                }
                else if (page != null)
                {
                    textRanks.Add(new TextRank(page.Title, 5));
                    textRanks.Add(new TextRank(page.TitleOnPage, 5));
                    textRanks.Add(new TextRank(page.SearchKeywords, 5));
                    textRanks.Add(new TextRank(page.Id, 3));
                    textRanks.Add(new TextRank(page.Keywords, 3));
                    textRanks.Add(new TextRank(page.SeoTitle, 2));
                }
                else
                {
                    textRanks.Add(new TextRank(item.Title, 5));
                    textRanks.Add(new TextRank(item.TitleOnPage, 5));
                    textRanks.Add(new TextRank(item.Id, 3));
                }
                KeywordsRankList.Add(item, SearchAgregator.GetKeywordsRankList(textRanks));
            }
            return KeywordsRankList[item];
        }
    }
}
