using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Audiochan.Application.Commons.Services
{
    public interface IStorageService
    {
        /// <summary>
        /// Create a pre-signed url to allow uploading (PUT request) an object into specified file name.
        /// Warning: Some storage provider may not have an implementation of this.
        /// </summary>
        /// <param name="bucket">Name of the bucket</param>
        /// <param name="container">Name of the container</param>
        /// <param name="blobName">Name of the object in the storage</param>
        /// <param name="expirationInMinutes">How long (in minutes) will the pre-signed url expires.</param>
        /// <param name="metadata">Any metadata to add to the object.</param>
        /// <returns>A string containing the pre-signed URL.</returns>
        string CreatePutPreSignedUrl(string bucket, string container, string blobName, int expirationInMinutes,
            Dictionary<string, string>? metadata = null);
        
        /// <summary>
        /// Create a pre-signed url to allow uploading (PUT request) an object into specified file name.
        /// </summary>
        /// <param name="bucket">Name of the bucket</param>
        /// <param name="blobName">Name of the object in the storage</param>
        /// <param name="expirationInMinutes">How long (in minutes) will the pre-signed url expires.</param>
        /// <param name="metadata">Any metadata to add to the object.</param>
        /// <returns>A string containing the pre-signed URL.</returns>
        string CreatePutPreSignedUrl(string bucket, string blobName, int expirationInMinutes,
            Dictionary<string, string>? metadata = null);

        /// <summary>
        /// Remove an object from the storage provider.
        /// </summary>
        /// <param name="bucket">Name of the bucket.</param>
        /// <param name="container">Name of the container that contains the object you want to remove.</param>
        /// <param name="blobName">Name of the object you want to remove.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns a task.</returns>
        Task RemoveAsync(string bucket, string container, string blobName, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Remove an object from the storage provider.
        /// </summary>
        /// <param name="bucket">Name of the bucket.</param>
        /// <param name="blobName">Name of the object you want to remove.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns a task.</returns>
        Task RemoveAsync(string bucket, string blobName, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Uploads a stream into an object in your storage provider.
        /// </summary>
        /// <param name="stream">The I/O stream you want to upload.</param>
        /// <param name="bucket">Name of the bucket.</param>
        /// <param name="container">Name of the container you want to upload the object.</param>
        /// <param name="blobName">Name of the object that will contain the data you want to upload.</param>
        /// <param name="metadata">A dictionary containing information about the object.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns a task.</returns>
        Task SaveAsync(Stream stream, string bucket, string container, string blobName,
            Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Uploads a stream into an object in your storage provider.
        /// </summary>
        /// <param name="stream">The I/O stream you want to upload.</param>
        /// <param name="bucket">Name of the bucket.</param>
        /// <param name="blobName">Name of the object that will contain the data you want to upload.</param>
        /// <param name="metadata">A dictionary containing information about the object.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns a task.</returns>
        Task SaveAsync(Stream stream, string bucket, string blobName,
            Dictionary<string, string>? metadata = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if the object exists inside your storage provider.
        /// </summary>
        /// <param name="bucket">Name of the bucket.</param>
        /// <param name="container">Name of the container that contains your object.</param>
        /// <param name="blobName">Name of the object.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns a task that returns a boolean, that determines if the object exists or not, once completed.</returns>
        Task<bool> ExistsAsync(string bucket, string container, string blobName,
            CancellationToken cancellationToken = default);
        
        /// <summary>
        /// Check if the object exists inside your storage provider.
        /// </summary>
        /// <param name="bucket">Name of the bucket.</param>
        /// <param name="blobName">Name of the object.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns a task that returns a boolean, that determines if the object exists or not, once completed.</returns>

        Task<bool> ExistsAsync(string bucket, string blobName,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Move the object from one place in your storage provider to another.
        /// </summary>
        /// <param name="sourceBucket">The bucket containing the object you want to remove.</param>
        /// <param name="sourceBlobName">The name of the object you want to remove. If the object is in a container, include it in the blob name.</param>
        /// <param name="targetBucket">The bucket where you want to remove the object.</param>
        /// <param name="targetBlobName">The name of object once you've moved it. Leaving it null or empty will use the original name.</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Returns a task.</returns>
        Task MoveBlobAsync(string sourceBucket,
            string sourceBlobName,
            string targetBucket,
            string? targetBlobName = null,
            CancellationToken cancellationToken = default);
    }
}