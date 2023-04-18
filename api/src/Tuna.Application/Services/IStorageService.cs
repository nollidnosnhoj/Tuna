using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Tuna.Application.Services;

public interface IStorageService
{
    /// <summary>
    ///     Create a pre-signed url to allow uploading (PUT request) an object into specified file name.
    /// </summary>
    /// <param name="bucket">Name of the bucket</param>
    /// <param name="blobName">Name of the object in the storage</param>
    /// <param name="expirationInMinutes">How long (in minutes) will the pre-signed url expires.</param>
    /// <param name="metadata">Any metadata to add to the object.</param>
    /// <returns>A string containing the pre-signed URL.</returns>
    string CreatePutPreSignedUrl(string bucket, string blobName, int expirationInMinutes,
        Dictionary<string, string>? metadata = null);

    /// <summary>
    ///     Remove an object from the storage provider.
    /// </summary>
    /// <param name="bucket">Name of the bucket.</param>
    /// <param name="blobName">Name of the object you want to remove.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns a task.</returns>
    Task RemoveAsync(string bucket, string blobName, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Check if the object exists inside your storage provider.
    /// </summary>
    /// <param name="bucket">Name of the bucket.</param>
    /// <param name="blobName">Name of the object.</param>
    /// <param name="cancellationToken"></param>
    /// <returns>Returns a task that returns a boolean, that determines if the object exists or not, once completed.</returns>
    Task<bool> ExistsAsync(string bucket, string blobName,
        CancellationToken cancellationToken = default);
}