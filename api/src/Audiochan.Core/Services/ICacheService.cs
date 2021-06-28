using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;

namespace Audiochan.Core.Services
{
    public interface ICacheService
    {
        Task<(bool cacheExists, TResponse? response)> GetAsync<TResponse>(string key, CancellationToken cancellationToken = default);
        Task<(bool cacheExists, TResponse? response)> GetAsync<TResponse>(ICacheOptions cacheOptions, CancellationToken cancellationToken = default);
        Task<bool> SetAsync<TValue>(string key, TValue value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
        Task<bool> SetAsync<TValue>(TValue value, ICacheOptions cacheOptions,
            CancellationToken cancellationToken = default);
        Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default);
        Task<bool> RemoveAsync(ICacheOptions cacheOptions, CancellationToken cancellationToken = default);
        Task<long> Increment(string key, long value = 1, CancellationToken cancellationToken = default);
        Task<long> Increment(long value, ICacheOptions cacheOptions, CancellationToken cancellationToken = default);
    }
}