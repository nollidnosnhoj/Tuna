using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;

namespace Audiochan.Infrastructure.Caching
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;

        public MemoryCacheService(IOptions<MemoryCacheOptions> memoryCacheOptions)
        {
            _cache = new MemoryCache(memoryCacheOptions.Value);
        }
        
        public Task<TResponse?> GetAsync<TResponse>(string key, CancellationToken cancellationToken = default)
        {
            _cache.TryGetValue<TResponse?>(key, out var response);
            return Task.FromResult(response);
        }

        public Task<bool> SetAsync<TValue>(string key, TValue value, TimeSpan? expiration = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                _cache.Set(key, value, expiration ?? TimeSpan.Zero);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return Task.FromResult(false);
            }
        }

        public Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                _cache.Remove(key);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                return Task.FromResult(false);
            }
        }

        public async Task<long> Increment(string key, long value = 1, CancellationToken cancellationToken = default)
        {
            var val = await GetAsync<long>(key, cancellationToken);
            val += value;
            await SetAsync(key, val, cancellationToken: cancellationToken);
            return val;
        }
    }
}