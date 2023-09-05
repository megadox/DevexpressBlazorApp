using System.Web;

namespace BatemBlazorApp
{
    class StaticAssetUtils
    {
        static string libraryPath = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

        public static string GetContentPath(string assetPath)
        {
            return $"./_content/{libraryPath}/{assetPath}";
        }

        public static string GetImagePath(string imageFileName)
        {
            return GetContentPath($"images/{imageFileName}");
        }
    }

    public static class ConnectionStringUtils
    {

        public static string GetNorthwindConnectionString(IConfiguration config)
        {
            return GetConnectionString(config, "NorthwindConnectionString");
        }
        public static string GetNorthwindSqliteConnectionString(IConfiguration config)
        {
            var dirPath = config.GetValue<string>("DataSourcesFolder");
            return $"Data Source={Path.Combine(dirPath, "nwind.db")}";
        }
        public static string GetHomesSqliteConnectionString(IConfiguration config)
        {
            var dirPath = config.GetValue<string>("DataSourcesFolder");
            return $"Data Source={Path.Combine(dirPath, "homes.db")}";
        }

        public static string GetIssuesConnectionString(IConfiguration config)
        {
            return GetConnectionString(config, "IssuesConnectionString");
        }
        public static string GetIssuesSqliteConnectionString(IConfiguration config)
        {
            var dirPath = config.GetValue<string>("DataSourcesFolder");
            return $"Data Source={Path.Combine(dirPath, "issue-list.db")}";
        }

        public static string GetWorlcitiesConnectionString(IConfiguration config)
        {
            return GetConnectionString(config, "WorldcitiesConnectionString");
        }
        public static string GetWorlcitiesSqliteConnectionString(IConfiguration config)
        {
            var dirPath = config.GetValue<string>("DataSourcesFolder");
            return $"Data Source={Path.Combine(dirPath, "worldcities.db")}";
        }

        public static string GetGridLargeDataConnectionString(IConfiguration config)
        {
            return GetConnectionString(config, "GridLargeDataConnectionString");
        }

        public static string GetPivotGridLargeDataConnectionString(IConfiguration config)
        {
            return GetConnectionString(config, "PivotGridLargeDataConnectionString");
        }

        static string GetConnectionString(IConfiguration config, string name)
        {
            var result = config.GetConnectionString(name);

            if (result == "{Remote demo database connection string}")
                return null;

            return result;
        }
    }

    public static class AppRenderUtils
    {
        private const string ExcludedPageSectionsQueryParameter = "excludedPageSections";

        public static bool PreventRenderDemoSection(string uri, string demoSectionId)
        {
            var queryString = (new Uri(uri)).Query;
            var queryCollection = HttpUtility.ParseQueryString(queryString);
            var excludedDemos = queryCollection.Get(ExcludedPageSectionsQueryParameter);
            if (!string.IsNullOrEmpty(excludedDemos) && excludedDemos.Split(',').Contains(demoSectionId))
            {
                return true;
            }

            return false;
        }

    }
}

