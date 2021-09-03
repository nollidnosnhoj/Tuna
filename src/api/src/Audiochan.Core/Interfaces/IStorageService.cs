using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Audiochan.Core.Interfaces
{
    public interface IStorageService
    {
        string CreatePutPreSignedUrl(string bucket, string container, string blobName, int expirationInMinutes,
            Dictionary<string, string>? metadata = null);
        
        string CreatePutPreSignedUrl(string bucket, string blobName, int expirationInMinutes,
            Dictionary<string, string>? metadata = null);

        Task RemoveAsync(string bucket, string container, string blobName,
            CancellationToken cancellationToken = default);
        
        Task RemoveAsync(string bucket, string blobName,
            CancellationToken cancellationToken = default);
        
        Task SaveAsync(Stream stream, string bucket, string container, string blobName,
            Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default);
        
        Task SaveAsync(Stream stream, string bucket, string blobName,
            Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default);

        Task<bool> ExistsAsync(string bucket, string container, string blobName,
            CancellationToken cancellationToken = default);
        
        Task<bool> ExistsAsync(string bucket, string blobName,
            CancellationToken cancellationToken = default);

        Task MoveBlobAsync(string sourceBucket,
            string sourceBlobName,
            string targetBucket,
            string? targetBlobName = null,
            CancellationToken cancellationToken = default);
    }
}