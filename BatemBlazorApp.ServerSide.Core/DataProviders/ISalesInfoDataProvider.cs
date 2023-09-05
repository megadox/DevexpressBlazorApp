using System.Runtime.InteropServices;
using BatemBlazorApp.Data;

namespace BatemBlazorApp.DataProviders
{
    [Guid("0FA3F7F9-ACDE-4B6C-9DAB-47589723818F")]
    public interface ISalesInfoDataProvider : IDataProvider
    {
        Task<IEnumerable<SaleInfo>> GetSalesAsync(CancellationToken ct = default);
        Task<IEnumerable<SaleInfo>> GetReducedSalesAsync(CancellationToken ct = default);
    }
}
