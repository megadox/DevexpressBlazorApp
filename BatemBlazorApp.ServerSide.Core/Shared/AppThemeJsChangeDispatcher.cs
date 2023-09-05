using DevExpress.Blazor.Internal;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using BatemBlazorApp.Configuration;
using BatemBlazorApp.Services;

namespace BatemBlazorApp.Shared
{
    public class AppThemeJsChangeDispatcher : ComponentBase, IAppThemeChangeRequestDispatcher, IAsyncDisposable
    {
        [Parameter]
        public string InitialThemeName { get; set; }

        [Inject]
        private ISafeJSRuntime JsRuntime { get; set; }

        [Inject]
        private AppThemeService Themes { get; set; }

        [Inject]
        private IAppStaticResourceService DemoStaticResourceService { get; set; }

        private AppTheme _pendingTheme;
        private IJSObjectReference _module;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Themes.ThemeChangeRequestDispatcher = this;

            if (Themes.ActiveTheme == null)
                Themes.SetActiveThemeByName(InitialThemeName);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
                _module = await JsRuntime.InvokeAsync<IJSObjectReference>("import", "./_content/BlazorDemo/lib/theme-controller.js");
        }

        public async void RequestThemeChange(AppTheme theme)
        {
            if (_pendingTheme == theme) return;

            _pendingTheme = theme;
            await _module.InvokeVoidAsync("ThemeController.setStylesheetLinks",
                Themes.GetBootstrapThemeCssUrl(theme),
                DemoStaticResourceService.GetUrlWithVersion(Themes.GetThemeCssUrl(theme)),
                Themes.GetHighlightJSThemeCssUrl(theme),
                DotNetObjectReference.Create(this));
        }

        [JSInvokable]
        public async Task ThemeLoadedAsync()
        {
            if (Themes.ThemeLoadNotifier != null)
            {
                await Themes.ThemeLoadNotifier.NotifyThemeLoadedAsync(_pendingTheme);
            }

            _pendingTheme = null;
        }

        public async ValueTask DisposeAsync()
        {
            if (_module != null)
                await _module.DisposeAsync();
        }
    }
}
