using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
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
        
        public Task<(bool cacheExists, TResponse? response)> GetAsync<TResponse>(string key, CancellationToken cancellationToken = default)
        {
            _cache.TryGetValue<TResponse?>(key, out var response);
            return Task.FromResult((response is not null, response));
        }

        public async Task<(bool cacheExists, TResponse? response)> GetAsync<TResponse>(ICacheOptions cacheOptions, CancellationToken cancellationToken = default)
        {
            return await GetAsync<TResponse>(cacheOptions.Key, cancellationToken);
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

        public async Task<bool> SetAsync<TValue>(TValue value, ICacheOptions cacheOptions, CancellationToken cancellationToken = default)
        {
            return await SetAsync(cacheOptions.Key, value, cacheOptions.Expiration, cancellationToken);
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

        public async Task<bool> RemoveAsync(ICacheOptions cacheOptions, CancellationToken cancellationToken = default)
        {
            return await RemoveAsync(cacheOptions.Key, cancellationToken);
        }

        public async Task<long> Increment(string key, long value = 1, CancellationToken cancellationToken = default)
        {
            var (_, val) = await GetAsync<long>(key, cancellationToken);
            val += value;
            await SetAsync(key, val, cancellationToken: cancellationToken);
            return val;
        }

        public async Task<long> Increment(long value, ICacheOptions cacheOptions, CancellationToken cancellationToken = default)
        {
            return await Increment(cacheOptions.Key, value, cancellationToken);
        }
    }
}