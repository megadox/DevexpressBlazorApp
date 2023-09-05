using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BatemBlazorApp.Configuration;
using BatemBlazorApp.ServerSide.Services;
using BatemBlazorApp.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;

namespace BatemBlazorApp.ServerSide
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {

            return Host.CreateDefaultBuilder(args).ConfigureWebHostDefaults(x => Configure(x, args));
        }

        private static void Configure(IWebHostBuilder webHostBuilder, string[] args)
        {
            webHostBuilder
                .UseConfiguration(
                    new ConfigurationBuilder()
                        .AddCommandLine(args)
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("ConnectionStrings.json", false, false)
                        .Build()
                )
                .ConfigureServices(ConfigureServices)
                .UseStaticWebAssets();
                


            static void ConfigureServices(WebHostBuilderContext context, IServiceCollection services)
            {
                services.AddOptions();
                services.AddHttpContextAccessor();

                services.AddSingleton<AppConfiguration>();
                services.AddScoped<AppThemeService>();
                services.AddScoped<IAppStaticResourceService, AppStaticResourceService>();
            }
        }
    }
}
