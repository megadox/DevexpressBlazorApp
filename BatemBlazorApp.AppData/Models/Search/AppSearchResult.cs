using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatemBlazorApp.AppData
{
    public class AppSearchResult : IComparable<AppSearchResult>
    {
        public AppSearchResult(AppItem item, int rank)
        {
            Item = item;
            Rank = rank;
        }

        public AppItem Item { get; private set; }

        public string Text { get; set; }
        public int Rank { get; set; }

        int IComparable<AppSearchResult>.CompareTo(AppSearchResult other)
        {
            return other.Rank.CompareTo(Rank);
        }
    }
}
