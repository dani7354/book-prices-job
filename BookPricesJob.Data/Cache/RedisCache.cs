using System.Text.Json;
using BookPricesJob.Application.Contract;
using Microsoft.Extensions.Caching.Distributed;

namespace BookPricesJob.Data.Cache;

public class RedisCache(IDistributedCache distributedCache) : ICache
{
    private readonly TimeSpan _defaultExpiry = TimeSpan.FromMinutes(5);

    private readonly IDistributedCache _distributedCache = distributedCache;

    public async Task<T?> GetAsync<T>(string key)
    {
        var value = await _distributedCache.GetStringAsync(key);

        return !string.IsNullOrEmpty(value) ? JsonSerializer.Deserialize<T>(value) : default;
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiry ?? _defaultExpiry
        };
        var serializedValue = JsonSerializer.Serialize(value);

        await _distributedCache.SetStringAsync(key, serializedValue, options);
    }

    public async Task RemoveAsync(string key)
    {
        await _distributedCache.RemoveAsync(key);
    }
}
