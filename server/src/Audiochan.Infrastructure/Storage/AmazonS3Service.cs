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
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Infrastructure.Storage.Options;
using Microsoft.Extensions.Options;

namespace Audiochan.Infrastructure.Storage
{
    public class AmazonS3Service : IStorageService
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

        public async Task RemoveAsync(string bucket, string container, string blobName,
            CancellationToken cancellationToken = default)
        {
            await RemoveAsync(bucket, GetKeyName(container, blobName), cancellationToken);
        }

        public async Task RemoveAsync(string bucket, string key, CancellationToken cancellationToken = default)
        {
            var deleteRequest = new DeleteObjectRequest {BucketName = bucket, Key = key};

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
            string bucket,
            string container,
            string blobName,
            Dictionary<string, string> metadata = null,
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
                    Key = GetKeyName(container, blobName),
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
                    BucketName = bucket,
                    Key = GetKeyName(container, blobName),
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
                Path = GetKeyName(container, blobName),
                Url = GetBlobUrl(bucket, container, blobName),
                ContentType = contentType
            };
        }

        public async Task<bool> ExistsAsync(string bucket, string container, string blobName,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    Key = GetKeyName(container, blobName),
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

        public string GetPresignedUrl(
            string method,
            string bucket,
            string container,
            string blobName,
            int expirationInMinutes,
            Dictionary<string, string> metadata = null)
        {
            var verb = method.ToLower() switch
            {
                "put" => HttpVerb.PUT,
                "head" => HttpVerb.HEAD,
                "delete" => HttpVerb.DELETE,
                "get" => HttpVerb.GET,
                _ => HttpVerb.GET
            };

            try
            {
                var expiration = _dateTimeProvider.Now.AddMinutes(5);
                var contentType = blobName.GetContentType();
                var presignedUrlRequest = new GetPreSignedUrlRequest
                {
                    BucketName = bucket,
                    Key = GetKeyName(container, blobName),
                    Expires = expiration,
                    ContentType = contentType,
                    Verb = verb
                };

                presignedUrlRequest.AddMetadataCollection(metadata);

                return _client.GetPreSignedURL(presignedUrlRequest);
            }
            catch (AmazonS3Exception ex)
            {
                throw new StorageException(ex.Message, ex);
            }
        }

        private string GetKeyName(string container, string blobName)
        {
            return $"${container}/${blobName}";
        }

        private string GetBlobUrl(string bucket, string container, string blobName)
        {
            return $"http://${bucket}.s3.amazonaws.com/${GetKeyName(container, blobName)}";
        }
    }

    public static class AmazonS3Extensions
    {
        public static void AddMetadataCollection(this PutObjectRequest request, Dictionary<string, string> data = null)
        {
            if (data?.Count > 0)
            {
                foreach (var (key, value) in data)
                {
                    request.Metadata.Add(key, value);
                }
            }
        }

        public static void AddMetadataCollection(this TransferUtilityUploadRequest request,
            Dictionary<string, string> data = null)
        {
            if (data?.Count > 0)
            {
                foreach (var (key, value) in data)
                {
                    request.Metadata.Add(key, value);
                }
            }
        }

        public static void AddMetadataCollection(this GetPreSignedUrlRequest request,
            Dictionary<string, string> data = null)
        {
            if (data?.Count > 0)
            {
                foreach (var (key, value) in data)
                {
                    request.Metadata.Add(key, value);
                }
            }
        }
    }
}