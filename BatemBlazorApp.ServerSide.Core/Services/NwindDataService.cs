using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BatemBlazorApp.Data.Northwind;
using BatemBlazorApp.DataProviders;

namespace BatemBlazorApp.Services
{
    public partial class NwindDataService
    {
        protected readonly INwindDataProvider _dataProvider;

        public NwindDataService(INwindDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }
    }
}
