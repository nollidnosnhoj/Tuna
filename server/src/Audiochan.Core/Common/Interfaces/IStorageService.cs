using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Models.Responses;

namespace Audiochan.Core.Common.Interfaces
{
    public interface IStorageService
    {
        string CreatePutPresignedUrl(string bucket, string container, string blobName, int expirationInMinutes,
            Dictionary<string, string> metadata = null);

        Task RemoveAsync(string bucket, string container, string blobName,
            CancellationToken cancellationToken = default);

        Task RemoveAsync(string bucket, string key, CancellationToken cancellationToken = default);

        Task<SaveBlobResponse> SaveAsync(Stream stream, string bucket, string container, string blobName,
            Dictionary<string, string> metadata = null, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(string bucket, string container, string blobName,
            CancellationToken cancellationToken = default);

        Task CopyBlobAsync(string sourceBucket,
            string sourceContainer,
            string sourceBlobName,
            string targetBucket,
            string targetContainer, 
            string targetKey = null,
            CancellationToken cancellationToken = default);
        
        Task MoveBlobAsync(string sourceBucket,
            string sourceContainer,
            string sourceBlobName,
            string targetBucket,
            string targetContainer, 
            string targetKey = null,
            CancellationToken cancellationToken = default);
    }
}