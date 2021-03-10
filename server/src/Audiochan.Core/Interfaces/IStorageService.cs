using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;

namespace Audiochan.Core.Interfaces
{
    public interface IStorageService
    {
        string GetPresignedUrl(string container, string blobName, string originalFileName, int expirationInMinutes,
            Dictionary<string, string> metadata = null);

        Task RemoveAsync(string container, string blobName, CancellationToken cancellationToken = default);
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);

        Task<SaveBlobResponse> SaveAsync(Stream stream, string container, string blobName,
            Dictionary<string, string> metadata = null, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(string container, string blobName, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
    }
}