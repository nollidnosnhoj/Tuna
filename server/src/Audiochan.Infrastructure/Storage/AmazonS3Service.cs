using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Audiochan.Core.Common.Exceptions;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Interfaces;
using Audiochan.Infrastructure.Storage.Extensions;
using Audiochan.Infrastructure.Storage.Options;
using Microsoft.Extensions.Options;

namespace Audiochan.Infrastructure.Storage
{
    public class AmazonS3Service : IStorageService
    {
        private readonly IAmazonS3 _client;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly string _bucket;
        private readonly long _chunkThreshold;
        private readonly string _url;

        public AmazonS3Service(IOptions<AmazonS3Options> amazonS3Options, IDateTimeProvider dateTimeProvider)
        {
            _dateTimeProvider = dateTimeProvider;
            _bucket = amazonS3Options.Value.Bucket;
            var region = RegionEndpoint.GetBySystemName(amazonS3Options.Value.Region);

            var s3Config = new AmazonS3Config
            {
                Timeout = ClientConfig.MaxTimeout,
                RegionEndpoint = region
            };

            var credentials = new BasicAWSCredentials(amazonS3Options.Value.PublicKey, amazonS3Options.Value.SecretKey);

            _chunkThreshold = amazonS3Options.Value.ChunkThreshold;
            _client = new AmazonS3Client(credentials, s3Config);
            _url = $"https://{_bucket}.s3.amazonaws.com";
        }

        public async Task RemoveAsync(string container, string blobName, CancellationToken cancellationToken = default)
        {
            var key = GetKeyName(container, blobName);

            await RemoveAsync(key, cancellationToken);
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            var deleteRequest = new DeleteObjectRequest {BucketName = _bucket, Key = key};

            try
            {
                await _client.DeleteObjectAsync(deleteRequest, cancellationToken);
            }
            catch (AmazonS3Exception ex)
            {
                throw new StorageException(ex.Message, ex);
            }
        }

        public async Task<SaveBlobResponse> SaveAsync(Stream stream,
            string container,
            string blobName,
            Dictionary<string, string> metadata = null,
            CancellationToken cancellationToken = default)
        {
            long? length = stream.CanSeek
                ? stream.Length
                : null;

            var threshold = Math.Min(_chunkThreshold, 5000000000);
            var key = GetKeyName(container, blobName);
            var blobUrl = string.Join('/', _url, key);
            var contentType = key.GetContentType();

            if (length >= threshold)
            {
                var transferUtility = new TransferUtility(_client);
                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = _bucket,
                    InputStream = stream,
                    PartSize = 6291456,
                    Key = key,
                    ContentType = contentType,
                    AutoCloseStream = true,
                    Headers = {ContentLength = length.Value},
                    CannedACL = S3CannedACL.PublicRead,
                };

                fileTransferUtilityRequest.AddMetadataCollection(metadata);

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
                    BucketName = _bucket,
                    Key = key,
                    InputStream = stream,
                    ContentType = contentType,
                    CannedACL = S3CannedACL.PublicRead,
                    AutoCloseStream = true
                };

                putRequest.AddMetadataCollection(metadata);

                try
                {
                    await _client.PutObjectAsync(putRequest, cancellationToken);
                }
                catch (AmazonS3Exception ex)
                {
                    throw new StorageException(ex.Message, ex);
                }
            }

            return new SaveBlobResponse
            {
                Url = blobUrl,
                Path = key,
                ContentType = contentType,
            };
        }

        public async Task<bool> ExistsAsync(string container, string blobName,
            CancellationToken cancellationToken = default)
        {
            var key = GetKeyName(container, blobName);

            return await ExistsAsync(key, cancellationToken);
        }

        public async Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new GetObjectMetadataRequest {Key = key, BucketName = _bucket,};
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

        public GetPresignedUrlResponse GetPresignedUrl(string container,
            string blobName,
            int expirationInMinutes,
            Dictionary<string, string> metadata = null)
        {
            try
            {
                var expiration = _dateTimeProvider.Now.AddMinutes(5);
                var key = GetKeyName(container, blobName);
                var contentType = key.GetContentType();
                var presignedUrlRequest = new GetPreSignedUrlRequest
                {
                    BucketName = _bucket,
                    Key = key,
                    Expires = expiration,
                    ContentType = contentType,
                    Verb = HttpVerb.PUT
                };

                presignedUrlRequest.AddMetadataCollection(metadata);

                var presignedUrl = _client.GetPreSignedURL(presignedUrlRequest);

                return new GetPresignedUrlResponse
                {
                    Url = presignedUrl
                };
            }
            catch (AmazonS3Exception ex)
            {
                throw new StorageException(ex.Message, ex);
            }
        }

        private static string GetKeyName(string container, string blobName)
        {
            var path = Path.Combine(container, blobName);
            return path.Replace(Path.DirectorySeparatorChar, '/');
        }
    }
}