using BatemBlazorApp.ServerSide.Core.Data;
using BatemBlazorApp.Services;
using DevExpress.Blazor.DocumentMetadata;
using BatemBlazorApp.Configuration;

namespace BatemBlazorApp
{
    public static class AppServiceCollectionExtensions
    {
        public static void AddAppServices(this IServiceCollection services, bool blazorWasm = false)
        {
            services.AddScoped<WeatherForecastService>();
            //services.AddScoped<RentInfoDataService>();
            services.AddScoped<ContosoRetailDataService>();
            //services.AddScoped<NwindDataService>();
            //services.AddScoped<HomesDataService>();
            //services.AddScoped<IssuesDataService>();
            //services.AddScoped<WorldcitiesDataService>();
            //services.AddDevExpressBlazor(opts => {
            //    opts.BootstrapVersion = DevExpress.Blazor.BootstrapVersion.v5;
            //}).AddSpellCheck(opts => {
            //    opts.FileProvider = new EmbeddedFileProvider(Assembly.GetExecutingAssembly(), "BlazorDemo");
            //    opts.MaxSuggestionCount = 6;
            //    opts.AddToDictionaryAction = (word, culture) => {
            //        //Write the selected word to a dictionary file
            //    };
            //    opts.Dictionaries.Add(new ISpellDictionary
            //    {
            //        DictionaryPath = "Data.Dictionaries.english.xlg",
            //        GrammarPath = "Data.Dictionaries.english.aff",
            //        Culture = "en-US"
            //    });
            //    opts.Dictionaries.Add(new Dictionary
            //    {
            //        DictionaryPath = "Data.Dictionaries.custom.dic",
            //        AlphabetPath = "Data.Dictionaries.english.txt",
            //        Culture = "en-US"
            //    });
            //});

            //if (blazorWasm)
            //{
            //    services.AddScoped<DevExpress.XtraReports.Services.IReportProviderAsync, DemoReportSourceWasm>();
            //    services.AddDevExpressWebAssemblyBlazorReportViewer();
            //}
            services.AddDocumentMetadata(ConfigureMetadata);
            services.AddSingleton<AppConfiguration>();

            static void ConfigureMetadata(IServiceProvider sp, IDocumentMetadataCollection metadataCollection)
            {
                sp.GetService<AppConfiguration>().ConfigureMetadata(metadataCollection);
            }
        }
    }
}
