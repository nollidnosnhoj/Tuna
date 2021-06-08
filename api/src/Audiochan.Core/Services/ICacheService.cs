using System;
using System.Threading;
using System.Threading.Tasks;

namespace Audiochan.Core.Services
{
    public interface ICacheService
    {
        Task<TResponse?> GetAsync<TResponse>(string key, CancellationToken cancellationToken = default);

        Task<bool> SetAsync<TValue>(string key, TValue value, TimeSpan? expiration = null,
            CancellationToken cancellationToken = default);

        Task<bool> RemoveAsync(string key, CancellationToken cancellationToken = default);


        Task<long> Increment(string key, long value = 1, CancellationToken cancellationToken = default);
    }
}