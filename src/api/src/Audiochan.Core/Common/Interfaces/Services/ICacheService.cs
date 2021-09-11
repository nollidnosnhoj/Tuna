using System;
using System.Threading;
using System.Threading.Tasks;

namespace Audiochan.Core.Common.Interfaces.Services
{
    public interface ICacheService
    {
        Task<TResponse?> GetAsync<TResponse>(string key, CancellationToken cancellationToken = default);
        Task<TResponse?> GetAsync<TResponse>(ICacheOptions cacheOptions, CancellationToken cancellationToken = default);
        Task<bool> SetAsync<TValue>(string key, TValue value, TimeSpan? expiration = null, CancellationToken cancellationToken = default);
        Task<bool> SetAsync<TValue>(TValue value, ICacheOptions cacheOptions,
            CancellationToken cancellationToken = default);
        Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default);
        Task<bool> RemoveAsync(ICacheOptions cacheOptions, CancellationToken cancellationToken = default);
        Task<long> IncrementAsync(string key, long value, CancellationToken cancellationToken = default);
        Task<long> IncrementAsync(long value, ICacheOptions cacheOptions, CancellationToken cancellationToken = default);
    }
}