using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Audiochan.Core.Common.Exceptions;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Infrastructure.Storage.Extensions;
using Audiochan.Infrastructure.Storage.Options;
using Microsoft.Extensions.Options;

namespace Audiochan.Infrastructure.Storage
{
    public class AmazonS3Service : IStorageService
    {
        private const string AwsDomain = "amazonaws.com";
        
        private readonly IAmazonS3 _client;
        private readonly string _bucket;
        private readonly long _chunkThreshold;
        private readonly string _url;

        public AmazonS3Service(IOptions<AmazonS3Options> iOptions)
        {
            var options = iOptions.Value;
            
            _bucket = options.Bucket;

            _url = string.IsNullOrWhiteSpace(options.Region)
                ? $"https://{_bucket}.s3.{AwsDomain}"
                : $"https://{_bucket}.s3-{options.Region}.{AwsDomain}";

            var region = RegionEndpoint.GetBySystemName(options.Region);

            var s3Config = new AmazonS3Config
            {
                Timeout = ClientConfig.MaxTimeout,
                RegionEndpoint = region
            };

            var credentials = new BasicAWSCredentials(options.PublicKey, options.SecretKey);

            _chunkThreshold = options.ChunkThreshold;
            _client = new AmazonS3Client(credentials, s3Config);
        }
        
        public async Task DeleteBlobAsync(string container, string blobName, 
            CancellationToken cancellationToken = default)
        {
            var key = GetKeyName(container, blobName);

            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _bucket,
                Key = key
            };

            try
            {
                await _client.DeleteObjectAsync(deleteRequest, cancellationToken);
            }
            catch (AmazonS3Exception ex)
            {
                throw new StorageException(ex.Message);
            }
        }

        public async Task SaveBlobAsync(string container, string blobName, Stream stream, 
            bool overwrite = true,
            CancellationToken cancellationToken = default)
        {
            long? length = stream.CanSeek ? stream.Length : null;

            var threshold = Math.Min(_chunkThreshold, 5000000000);

            if (length >= threshold)
            {
                var transferUtility = new TransferUtility(_client);
                var fileTransferUtilityRequest = new TransferUtilityUploadRequest
                {
                    BucketName = _bucket,
                    InputStream = stream,
                    PartSize = 6291456,
                    Key = GetKeyName(container, blobName),
                    ContentType = blobName.GetContentType(),
                    AutoCloseStream = true,
                    Headers = {ContentLength = length.Value},
                    CannedACL = S3CannedACL.PublicRead
                };

                try
                {
                    await transferUtility.UploadAsync(fileTransferUtilityRequest, cancellationToken);
                }
                catch (AmazonS3Exception ex)
                {
                    throw new StorageException(ex.Message);
                }
            }
            else
            {
                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucket,
                    Key = GetKeyName(container, blobName),
                    InputStream = stream,
                    ContentType = blobName.GetContentType(),
                    CannedACL = S3CannedACL.PublicRead,
                    AutoCloseStream = true
                };

                try
                {
                    await _client.PutObjectAsync(putRequest, cancellationToken);
                }
                catch (AmazonS3Exception ex)
                {
                    throw new StorageException(ex.Message);
                }
            }
        }

        public async Task<BlobDto> GetBlobAsync(string container, string blobName,
            CancellationToken cancellationToken = default)
        {
            var key = GetKeyName(container, blobName);

            try
            {
                var getRequest = new GetObjectMetadataRequest
                {
                    BucketName = _bucket,
                    Key = key
                };

                var response = await _client.GetObjectMetadataAsync(getRequest, cancellationToken);

                return new BlobDto(true, container, blobName, $"{_url}/{key}", response.ContentLength);
            }
            catch (AmazonS3Exception ex)
            {
                throw new StorageException(ex.Message);
            }
        }

        private static string GetKeyName(string container, string blobName)
        {
            container = container.Replace('\\', '/');
            
            return string.IsNullOrWhiteSpace(container)
                ? blobName
                : $"{container}/{blobName}";
        }
    }
}