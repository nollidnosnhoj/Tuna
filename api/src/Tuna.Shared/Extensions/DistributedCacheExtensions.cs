using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

namespace Tuna.Shared.Extensions
{
    public static class DistributedCacheExtensions
    {
        public static async Task<T?> GetAsync<T>(this IDistributedCache cache, string key, JsonSerializerOptions? jsonSerializerOptions = null,
            CancellationToken cancellationToken = default)
        {
            var bytes = await cache.GetAsync(key, cancellationToken);
            if (bytes is null) return default;
            return JsonSerializer.Deserialize<T>(bytes, jsonSerializerOptions ?? new JsonSerializerOptions());
        }

        public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value,
            CancellationToken cancellationToken = default)
        {
            await cache.SetAsync(key, value, 
                new DistributedCacheEntryOptions(), 
                new JsonSerializerOptions(),
                cancellationToken);
        }

        public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value,
            DistributedCacheEntryOptions cachingOptions,
            CancellationToken cancellationToken = default)
        {
            await cache.SetAsync(key, value, cachingOptions, new JsonSerializerOptions(), cancellationToken);
        }
        
        public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value,
            JsonSerializerOptions jsonSerializerOptions,
            CancellationToken cancellationToken = default)
        {
            await cache.SetAsync(key, value, new DistributedCacheEntryOptions(), jsonSerializerOptions, cancellationToken);
        }

        public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value,
            DistributedCacheEntryOptions cachingOptions,
            JsonSerializerOptions jsonSerializerOptions,
            CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(value, jsonSerializerOptions);
            await cache.SetStringAsync(key, json, cachingOptions, cancellationToken);
        }

        public static async Task<T?> GetOrCreateAsync<T>(this IDistributedCache cache, string key,
            Func<Task<T?>> factory, 
            DistributedCacheEntryOptions? cachingOptions = null,
            JsonSerializerOptions? jsonSerializerOptions = null,
            CancellationToken cancellationToken = default)
        {
            var cacheContent = await cache.GetAsync<T>(key, jsonSerializerOptions, cancellationToken);
            if (cacheContent is not null) return cacheContent;
            var value = await factory();
            if (value is null) return default;
            await cache.SetAsync(key, 
                value, 
                cachingOptions ?? new DistributedCacheEntryOptions(), 
                jsonSerializerOptions ?? new JsonSerializerOptions(), 
                cancellationToken);
            return value;
        }
    }
}