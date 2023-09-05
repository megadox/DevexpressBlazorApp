namespace BatemBlazorApp.DataProviders
{
    public interface IDataProvider
    {
        Task<IObservable<int>> GetLoadingStateAsync();
    }
}

