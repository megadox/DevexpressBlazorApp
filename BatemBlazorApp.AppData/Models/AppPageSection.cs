using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatemBlazorApp.AppData
{
    public class AppPageSection : AppItem
    {
        public override string GetUrl()
        {
            return ParentPage.GetUrl() + "#" + Id;
        }
        public override AppItem[] GetChildItems()
        {
            return Array.Empty<AppPageSection>();
        }
        public override string GetRazorFilesFolder()
        {
            return ParentPage?.GetRazorFilesFolder();
        }
        public override string GetDescriptionFilesFolder()
        {
            return ParentPage?.GetDescriptionFilesFolder();
        }
    }
}
