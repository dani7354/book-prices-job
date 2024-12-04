using BookPricesJob.Application.Contract;

namespace BookPricesJob.Test.Setup;

public class FakeCache : ICache
{
    public Task<T?> GetAsync<T>(string key)
    {
        return Task.FromResult(default(T));
    }

    public Task RemoveAsync(string key)
    {
        return Task.CompletedTask;
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        return  Task.CompletedTask;
    }
}
