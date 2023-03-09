using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Audiochan.Core.Exceptions;
using Audiochan.Shared.Extensions;
using Audiochan.Core.Services;
using Microsoft.Extensions.Options;

namespace Audiochan.Infrastructure.Storage.AmazonS3;

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
}