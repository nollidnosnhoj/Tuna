using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Interfaces.Services;
using StackExchange.Redis;

namespace Audiochan.Infrastructure.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _cache;

        public RedisCacheService(IConnectionMultiplexer connectionMultiplexer)
        {
            _cache = connectionMultiplexer.GetDatabase();
        }

        public async Task<(bool cacheExists, TResponse? response)> GetAsync<TResponse>(string key, CancellationToken cancellationToken = default)
        {
            var value = (string?)await _cache.StringGetAsync((RedisKey) key);
            var exists = value is not null;
            var response = exists 
                ?  JsonSerializer.Deserialize<TResponse>(value!)
                : default;
            return (exists, response);
        }

        public async Task<(bool cacheExists, TResponse? response)> GetAsync<TResponse>(ICacheOptions cacheOptions, CancellationToken cancellationToken = default)
        {
            return await GetAsync<TResponse>(cacheOptions.Key, cancellationToken);
        }

        public async Task<bool> SetAsync<TValue>(string key, TValue value, TimeSpan? expiration = null,
            CancellationToken cancellationToken = default)
        {
            var json = JsonSerializer.Serialize(value);
            return await _cache.StringSetAsync(key, json, expiration);
        }

        public async Task<bool> SetAsync<TValue>(TValue value, ICacheOptions cacheOptions, CancellationToken cancellationToken = default)
        {
            return await SetAsync(cacheOptions.Key, value, cacheOptions.Expiration, cancellationToken);
        }

        public async Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            return await _cache.KeyDeleteAsync(key);
        }

        public async Task<bool> RemoveAsync(ICacheOptions cacheOptions, CancellationToken cancellationToken = default)
        {
            return await RemoveAsync(cacheOptions.Key, cancellationToken);
        }

        public async Task<long> Increment(string key, long value = 1, CancellationToken cancellationToken = default)
        {
            return await _cache.StringIncrementAsync(key, value);
        }

        public async Task<long> Increment(long value, ICacheOptions cacheOptions, CancellationToken cancellationToken = default)
        {
            return await Increment(cacheOptions.Key, value, cancellationToken);
        }
    }
}