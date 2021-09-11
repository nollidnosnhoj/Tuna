using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Interfaces.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Audiochan.Infrastructure.Caching
{
    public class InMemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public InMemoryCacheService(IOptions<MemoryCacheOptions> memoryCacheOptions)
        {
            _cache = new MemoryCache(memoryCacheOptions.Value);
        }
        
        public Task<TResponse?> GetAsync<TResponse>(string key, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Get<TResponse>(key));
        }

        public Task<TResponse?> GetAsync<TResponse>(ICacheOptions cacheOptions, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Get<TResponse>(cacheOptions.Key));
        }

        public Task<bool> SetAsync<TValue>(string key, TValue value, TimeSpan? expiration = null,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Set(key, value, expiration));
        }

        public Task<bool> SetAsync<TValue>(TValue value, ICacheOptions cacheOptions, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Set(cacheOptions.Key, value, cacheOptions.Expiration));
        }

        public Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Remove(key));
        }

        public Task<bool> RemoveAsync(ICacheOptions cacheOptions, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Remove(cacheOptions.Key));
        }

        public Task<long> IncrementAsync(string key, long value, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Increment(key, value));
        }

        public Task<long> IncrementAsync(long value, ICacheOptions cacheOptions, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(Increment(cacheOptions.Key, value));
        }
        
        private TResponse? Get<TResponse>(string key)
        {
            return _cache.TryGetValue<TResponse>(key, out var response)
                ? response
                : default;
        }

        private bool Set<TValue>(string key, TValue value, TimeSpan? expiration = null)
        {
            try
            {
                _cache.Set(key, value, expiration ?? TimeSpan.Zero);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool Remove(string key)
        {
            try
            {
                _cache.Remove(key);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private long Increment(string key, long value = 1)
        {
            if (_cache.TryGetValue<long>(key, out var val))
            {
                value += val;
            }

            _cache.Set(key, value);
            return value;
        }
    }
}