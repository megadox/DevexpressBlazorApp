namespace BatemBlazorApp.Configuration
{
    public class AppVersion : IAppVersion
    {
        public string Version { get; }

        public AppVersion(string version)
        {
            Version = version;
        }
    }

    public interface IAppVersion
    {
        string Version { get; }
    }
}
