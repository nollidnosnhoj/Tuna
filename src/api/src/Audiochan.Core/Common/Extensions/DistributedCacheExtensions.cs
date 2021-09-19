using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Audiochan.Core.Common.Extensions
{
    public static class DistributedCacheExtensions
    {
        public static async Task<T?> GetAsync<T>(this IDistributedCache cache, string key, 
            CancellationToken cancellationToken = default)
        {
            var bytes = await cache.GetAsync(key, cancellationToken);
            if (bytes is null) return default;
            return JsonSerializer.Deserialize<T>(bytes);
        }
        
        public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, 
            CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(value);
            await cache.SetStringAsync(key, json, cancellationToken);
        }

        public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, 
            DistributedCacheEntryOptions options,  CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(value);
            await cache.SetStringAsync(key, json, options, cancellationToken);
        }

        public static async Task<T?> GetOrCreateAsync<T>(this IDistributedCache cache, string key,
            Func<Task<T?>> factory, DistributedCacheEntryOptions options, CancellationToken cancellationToken = default)
        {
            var cacheContent = await cache.GetAsync<T>(key, cancellationToken);
            if (cacheContent is not null) return cacheContent;
            var value = await factory();
            if (value is null) return default;
            await cache.SetAsync(key, value, options, cancellationToken);
            return value;
        }
    }
}