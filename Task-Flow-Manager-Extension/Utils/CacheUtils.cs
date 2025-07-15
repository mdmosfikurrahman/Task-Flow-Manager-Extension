using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Task_Flow_Manager_Extension.Utils;

public static class CacheUtils
{
    public static async Task<T?> GetOrSetAsync<T>(
        this IDistributedCache cache,
        string key,
        Func<Task<T>> factory,
        TimeSpan? expiration = null)
    {
        var cached = await cache.GetStringAsync(key);
        if (!string.IsNullOrEmpty(cached))
            return JsonSerializer.Deserialize<T>(cached);

        var value = await factory();
        var serialized = JsonSerializer.Serialize(value);

        await cache.SetStringAsync(key, serialized, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
        });

        return value;
    }

    public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, TimeSpan? expiration = null)
    {
        await cache.RemoveAsync(key);

        var serialized = JsonSerializer.Serialize(value);
        await cache.SetStringAsync(key, serialized, new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(5)
        });
    }


    public static async Task<T?> GetAsync<T>(this IDistributedCache cache, string key)
    {
        var cached = await cache.GetStringAsync(key);
        return string.IsNullOrEmpty(cached) ? default : JsonSerializer.Deserialize<T>(cached);
    }
}