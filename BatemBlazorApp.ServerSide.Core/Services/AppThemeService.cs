﻿using BatemBlazorApp.Configuration;
using System.Collections.Concurrent;

namespace BatemBlazorApp.Services
{

    public interface IAppThemeChangeRequestDispatcher
    {
        void RequestThemeChange(AppTheme theme);
    }

    public interface IAppThemeLoadNotifier
    {
        Task NotifyThemeLoadedAsync(AppTheme theme);
    }

    public class AppThemeService
    {
        public static bool EnableNewBlazorThemes = true;
        readonly Dictionary<string, string> newBlazorThemesMapping = new()
        {
            ["blazing-berry"] = "blazing-berry.bs5",
            ["blazing-dark"] = "blazing-dark.bs5",
            ["office-white"] = "office-white.bs5",
            ["purple"] = "purple.bs5",
        };
        private AppTheme _activeTheme;
#if SERVER_BLAZOR
        public const string ThemeCookieKey = "DXBZCurrentTheme";
#else
        public const string ThemeCookieKey = "DXBZCurrentWasmTheme";
#endif
        const string DefaultThemeName = "blazing-berry";
        readonly Dictionary<string, string> HighlightJSThemes = new Dictionary<string, string>() {
            { DefaultThemeName, "default" },
            { "blazing-dark", "androidstudio" },
            { "cyborg", "androidstudio" }
        };

        public IAppThemeChangeRequestDispatcher ThemeChangeRequestDispatcher { get; set; }

        public IAppThemeLoadNotifier ThemeLoadNotifier { get; set; }

        public AppThemeService()
        {
            ResourcesReadyState = new ConcurrentDictionary<string, TaskCompletionSource<bool>>();
            ThemeSets = CreateSets(this);
        }

        public ConcurrentDictionary<string, TaskCompletionSource<bool>> ResourcesReadyState { get; }
        public List<AppThemeSet> ThemeSets { get; }
        public AppTheme ActiveTheme => _activeTheme;
        public AppTheme DefaultTheme
        {
            get { return ThemeSets.SelectMany(ts => ts.Themes).FirstOrDefault(t => t.Name == DefaultThemeName); }
        }

        public string GetThemeCssUrl(AppTheme theme)
        {
            if (EnableNewBlazorThemes)
            {
                if (this.newBlazorThemesMapping.ContainsKey(theme.Name))
                    return $"_content/DevExpress.Blazor.Themes/{this.newBlazorThemesMapping.GetValueOrDefault(theme.Name)}.min.css";
                return $"_content/DevExpress.Blazor.Themes/bootstrap-external.bs5.min.css";
            }
            return GetBootstrapThemeCssUrl(theme);
        }
        public string GetBootstrapThemeCssUrl(AppTheme theme)
        {
            if (!EnableNewBlazorThemes || theme.IsBootstrapNative)
            {
                return $"_content/BlazorDemo/css/switcher-resources/themes/{theme.Name}/bootstrap.min.css";
            }
            return null;
        }
        public string GetActiveThemeCssUrl()
        {
            return GetThemeCssUrl(ActiveTheme);
        }
        public string GetActiveBootstrapThemeCssUrl()
        {
            return GetBootstrapThemeCssUrl(ActiveTheme);
        }
        public string GetHighlightJSThemeCssUrl(AppTheme theme)
        {
            var highlightjsTheme = HighlightJSThemes[DefaultThemeName];
            if (HighlightJSThemes.ContainsKey(theme.Name))
                highlightjsTheme = HighlightJSThemes[theme.Name];
            return $"https://cdnjs.cloudflare.com/ajax/libs/highlight.js/9.15.6/styles/{highlightjsTheme}.min.css";
        }
        public string GetActiveHighlightJSThemeCssUrl()
        {
            return GetHighlightJSThemeCssUrl(ActiveTheme);
        }

        public void SetActiveThemeByName(string themeName)
        {
            var theme = FindThemeByName(themeName);
            if (theme != null)
                _activeTheme = theme;
            else
                _activeTheme = DefaultTheme;
        }
        private AppTheme FindThemeByName(string themeName)
        {
            var themes = ThemeSets.SelectMany(ts => ts.Themes);
            foreach (var theme in themes)
            {
                if (theme.Name == themeName)
                    return theme;
            }
            return null;
        }

        private static List<AppThemeSet> CreateSets(AppThemeService config)
        {
            return new List<AppThemeSet>() {
                new AppThemeSet("DevExpress Themes", "blazing-berry", "blazing-dark", "purple", "office-white"),
                new AppThemeSet("Bootstrap Themes", "default", "cerulean", "cyborg", "flatly", "journal", "litera", "lumen", "lux", "pulse", "simplex", "solar", "superhero", "united", "yeti")
            };
        }
    }
}
