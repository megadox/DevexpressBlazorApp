using BatemBlazorApp.Data;
using System.Runtime.InteropServices;

namespace BatemBlazorApp.DataProviders
{
    [Guid("21476BD9-7B6C-4E0E-961D-56E80C3EFAF7")]
    public interface IContosoRetailDataProvider
    {
        IQueryable<Sale> GetSales();
    }
}
