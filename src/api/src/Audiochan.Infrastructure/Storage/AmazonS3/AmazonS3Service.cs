using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Audiochan.Core.Commons.Extensions;
using Audiochan.Core.Commons.Services;
using Microsoft.Extensions.Options;

namespace Audiochan.Infrastructure.Storage.AmazonS3
{
    internal class AmazonS3Service : IStorageService
    {
        private readonly IAmazonS3 _client;
        private readonly IDateTimeProvider _dateTimeProvider;

        public AmazonS3Service(IOptions<AmazonS3Settings> amazonS3Options, IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
            var region = RegionEndpoint.GetBySystemName(amazonS3Options.Value.Region);

            var s3Config = new AmazonS3Config
            {
                Timeout = ClientConfig.MaxTimeout,
                RegionEndpoint = region
            };

            var credentials = new BasicAWSCredentials(amazonS3Options.Value.PublicKey, amazonS3Options.Value.SecretKey);

            _client = new AmazonS3Client(credentials, s3Config);
        }
        
        public string CreatePutPreSignedUrl(
            string bucket,
            string container,
            string blobName,
            int expirationInMinutes,
            Dictionary<string, string>? metadata = null)
        {
            return CreatePutPreSignedUrl(bucket, GetKeyName(container, blobName), expirationInMinutes, metadata);
        }

        public string CreatePutPreSignedUrl(string bucket, string blobName, int expirationInMinutes, 
            Dictionary<string, string>? metadata = null)
        {
            try
            {
                var contentType = blobName.GetContentType();
                var presignedUrlRequest = new GetPreSignedUrlRequest
                {
                    BucketName = bucket,
                    Key = blobName,
                    Expires = _dateTimeProvider.Now.AddMinutes(expirationInMinutes),
                    ContentType = contentType,
                    Verb = HttpVerb.PUT,
                };

                presignedUrlRequest.Metadata.AddMetadata(metadata);

                return _client.GetPreSignedURL(presignedUrlRequest);
            }
            catch (AmazonS3Exception ex)
            {
                throw new StorageException(ex.Message, ex);
            }
        }

        public async Task RemoveAsync(string bucket, string container, string blobName,
            CancellationToken cancellationToken = default)
        {
            await RemoveAsync(bucket, GetKeyName(container, blobName), cancellationToken);
        }
        
        public async Task RemoveAsync(string bucket, string blobName, CancellationToken cancellationToken = default)
        {
            var deleteRequest = new DeleteObjectRequest {BucketName = bucket, Key = blobName};

            try
            {
                await _client.DeleteObjectAsync(deleteRequest, cancellationToken);
            }
            catch (AmazonS3Exception ex)
            {
                throw new StorageException(ex.Message, ex);
            }
        }

        public async Task SaveAsync(Stream stream,
            string bucket,
            string container,
            string blobName,
            Dictionary<string, string>? metadata = null,
            CancellationToken cancellationToken = default)
        {
            await SaveAsync(stream, bucket, GetKeyName(container, blobName), metadata, cancellationToken);
        }

        public async Task SaveAsync(Stream stream, string bucket, string blobName, Dictionary<string, string>? metadata = null,
            CancellationToken cancellationToken = default)
        {
            long? length = stream.CanSeek
                ? stream.Length
                : null;

            var contentType = blobName.GetContentType();

            if (length >= 5000000000)
            {
                var transferUtility = new TransferUtility(_client);
                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = bucket,
                    InputStream = stream,
                    PartSize = 6291456,
                    Key = blobName,
                    ContentType = contentType,
                    AutoCloseStream = true,
                    Headers = {ContentLength = length.Value},
                    CannedACL = S3CannedACL.PublicRead,
                };

                fileTransferUtilityRequest.Metadata.AddMetadata(metadata);

                try
                {
                    await transferUtility.UploadAsync(fileTransferUtilityRequest, cancellationToken);
                }
                catch (AmazonS3Exception ex)
                {
                    throw new StorageException(ex.Message, ex);
                }
            }
            else
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = bucket,
                    Key = blobName,
                    InputStream = stream,
                    ContentType = contentType,
                    CannedACL = S3CannedACL.PublicRead,
                    AutoCloseStream = true
                };

                putRequest.Metadata.AddMetadata(metadata);

                try
                {
                    await _client.PutObjectAsync(putRequest, cancellationToken);
                }
                catch (AmazonS3Exception ex)
                {
                    throw new StorageException(ex.Message, ex);
                }
            }
        }

        public async Task<bool> ExistsAsync(string bucket, string container, string blobName,
            CancellationToken cancellationToken = default)
        {
            return await ExistsAsync(bucket, GetKeyName(container, blobName), cancellationToken);
        }

        public async Task<bool> ExistsAsync(string bucket, string blobName, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    Key = blobName,
                    BucketName = bucket,
                };
                await _client.GetObjectMetadataAsync(request, cancellationToken);
                return true;
            }
            catch (AmazonS3Exception ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                    return false;
                throw new StorageException(ex.Message, ex);
            }
        }

        public async Task MoveBlobAsync(string sourceBucket,
            string sourceBlobName,
            string targetBucket,
            string? targetKey = null,
            CancellationToken cancellationToken = default)
        {
            await CopyBlobAsync(sourceBucket,
                sourceBlobName, 
                targetBucket,
                targetKey,
                cancellationToken);

            await RemoveAsync(sourceBucket, sourceBlobName, cancellationToken);
        }

        public async Task MoveBlobAsync(string sourceBucket,
            string sourceContainer,
            string sourceBlobName,
            string targetBucket,
            string targetContainer,
            string targetKey,
            CancellationToken cancellationToken = default)
        {
            await CopyBlobAsync(sourceBucket,
                GetKeyName(sourceContainer, sourceBlobName),
                targetBucket,
                GetKeyName(targetContainer, targetKey),
                cancellationToken);

            await RemoveAsync(sourceBucket, GetKeyName(sourceContainer, sourceBlobName), cancellationToken);
        }

        private string GetKeyName(string container, string blobName)
        {
            return $"{container}/{blobName}";
        }
        
        private async Task CopyBlobAsync(string sourceBucket, 
            string sourceBlobName, 
            string targetBucket, 
            string? targetBlobName = null,
            CancellationToken cancellationToken = default)
        {
            var newTargetKey = targetBlobName ?? sourceBlobName;

            try
            {
                var request = new CopyObjectRequest
                {
                    SourceBucket = sourceBucket,
                    SourceKey = sourceBlobName,
                    DestinationBucket = targetBucket,
                    DestinationKey = newTargetKey,
                };

                var response = await _client.CopyObjectAsync(request, cancellationToken);

                if (response.HttpStatusCode != HttpStatusCode.OK)
                    throw new StorageException("Copy object failed.");
            }
            catch (AmazonS3Exception ex)
            {
                throw new StorageException(ex.Message, ex);
            }
        }
    }

    public static class AmazonS3Extensions
    {
        public static void AddMetadata(this MetadataCollection metadataCollection,
            IDictionary<string, string>? inputMeta)
        {
            if (inputMeta == null) return;

            foreach (var (key, value) in inputMeta)
                metadataCollection[key] = value;
        }

        public static IDictionary<string, string> ToMetadata(this MetadataCollection metadataCollection)
        {
            return metadataCollection.Keys
                .ToDictionary(k => k.Replace("x-amz-meta-", string.Empty),
                    k => metadataCollection[k]);
        }
    }
}