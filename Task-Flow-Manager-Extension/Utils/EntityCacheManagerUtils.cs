using Microsoft.Extensions.Caching.Distributed;

namespace Task_Flow_Manager_Extension.Utils;

public class EntityCacheManagerUtils<T>(IDistributedCache cache, string prefix, TimeSpan? expiration = null)
{
    private readonly TimeSpan _expiration = expiration ?? TimeSpan.FromMinutes(5);

    private string IdKey(object id) => $"{prefix}_{id}";
    private string AllKey() => $"{prefix}_all";

    public Task<T?> GetByIdAsync(object id, Func<Task<T>> factory)
        => cache.GetOrSetAsync(IdKey(id), factory, _expiration);

    public Task SetByIdAsync(object id, T value)
        => cache.SetAsync(IdKey(id), value, _expiration);

    public Task RemoveByIdAsync(object id)
        => cache.RemoveAsync(IdKey(id));

    public Task<List<T>?> GetAllAsync(Func<Task<List<T>>> factory)
        => cache.GetOrSetAsync(AllKey(), factory, _expiration);

    public Task SetAllAsync(List<T> values)
        => cache.SetAsync(AllKey(), values, _expiration);

    public Task RemoveAllAsync()
        => cache.RemoveAsync(AllKey());
}