using BatemBlazorApp.DataProviders;

namespace BatemBlazorApp.Services
{
    public partial class ContosoRetailDataService
    {
        protected readonly IContosoRetailDataProvider _dataProvider;

        public ContosoRetailDataService(IContosoRetailDataProvider dataProvider)
        {
            _dataProvider = dataProvider;
        }

    }
}
