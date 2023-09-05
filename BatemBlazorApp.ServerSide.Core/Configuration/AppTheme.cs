using System.Globalization;

namespace BatemBlazorApp.Configuration
{
    public class AppTheme
    {
        public string Name { get; }
        public string Title { get { return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(Name.Replace("-", " ")); } }
        public string IconCssClass { get { return Name.ToLower(); } }
        public bool IsBootstrapNative { get; }
        public string GetCssClass(bool isActive) => isActive ? "active" : "text-body";
        public AppTheme(string name, bool isBootstrapNative)
        {
            Name = name;
            IsBootstrapNative = isBootstrapNative;
        }
    }

    public class AppThemeSet
    {
        static readonly HashSet<string> BuiltInThemes = new HashSet<string>() {
            "blazing-berry", "blazing-dark", "purple", "office-white"
        };
        public string Title { get; }
        public AppTheme[] Themes { get; }
        public AppThemeSet(string title, params string[] themes)
        {
            Title = title;
            Themes = themes.Select(CreateTheme).ToArray();


            AppTheme CreateTheme(string name)
            {
                bool isBootstrapNative = !BuiltInThemes.Contains(name);
                return new AppTheme(name, isBootstrapNative);
            }
        }
    }
}
