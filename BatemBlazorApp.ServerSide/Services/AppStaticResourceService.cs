using BatemBlazorApp.Services;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace BatemBlazorApp.ServerSide.Services
{
    public class AppStaticResourceService : IAppStaticResourceService
    {
        public AppStaticResourceService(IHttpContextAccessor contextAccessor)
        {
            ContextAccessor = contextAccessor;
        }

        private IHttpContextAccessor ContextAccessor { get; set; }

        public string GetUrlWithVersion(string url)
        {
            var context = ContextAccessor.HttpContext;
            if (context != null)
            {
                IFileVersionProvider fileVersionProvider = (IFileVersionProvider)context.RequestServices.GetService(typeof(IFileVersionProvider));
                url = fileVersionProvider.AddFileVersionToPath(context.Request.PathBase, url);
            }
            return url;
        }
    }
}
