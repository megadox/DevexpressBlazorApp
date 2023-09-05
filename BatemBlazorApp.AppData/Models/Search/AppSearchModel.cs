namespace BatemBlazorApp.AppData
{
    public class AppSearchModel
    {
        public AppSynonymsSearchModel Synonyms { get; set; }
        public AppExclusionsSearchModel Exclusions { get; set; }
    }

    public class AppExclusionsSearchModel
    {
        public string Words { get; set; }
        public string Prefixes { get; set; }
        public string Postfixes { get; set; }
    }
    public class AppSynonymsSearchModel
    {
        public string[] Groups { get; set; }
    }
}
