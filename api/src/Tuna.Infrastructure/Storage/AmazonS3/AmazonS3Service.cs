using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using Tuna.Application.Exceptions;
using Tuna.Application.Services;
using Tuna.Shared.Extensions;

namespace Tuna.Infrastructure.Storage.AmazonS3;

internal class AmazonS3Service : IStorageService
{
    private readonly IAmazonS3 _client;
    private readonly IClock _clock;

    public AmazonS3Service(IOptions<AWSSettings> amazonS3Options, IClock clock)
    {
        _clock = clock;
        var region = RegionEndpoint.GetBySystemName(amazonS3Options.Value.Region);

        var s3Config = new AmazonS3Config
        {
            Timeout = ClientConfig.MaxTimeout,
            RegionEndpoint = region
        };

        var credentials = new BasicAWSCredentials(amazonS3Options.Value.PublicKey, amazonS3Options.Value.SecretKey);

        _client = new AmazonS3Client(credentials, s3Config);
    }

    public string CreateUploadUrl(string bucket, string blobName, TimeSpan expiration,
        Dictionary<string, string>? metadata = null)
    {
        try
        {
            var contentType = blobName.GetContentType();
            var request = new GetPreSignedUrlRequest
            {
                BucketName = bucket,
                Key = blobName,
                Expires = _clock.UtcNow.Add(expiration),
                ContentType = contentType,
                Verb = HttpVerb.PUT
            };

            request.Metadata.AddMetadata(metadata);

            return _client.GetPreSignedURL(request);
        }
        catch (AmazonS3Exception ex)
        {
            throw new StorageException(ex.Message, ex);
        }
    }

    public async Task RemoveAsync(string bucket, string blobName, CancellationToken cancellationToken = default)
    {
        var deleteRequest = new DeleteObjectRequest { BucketName = bucket, Key = blobName };

        try
        {
            await _client.DeleteObjectAsync(deleteRequest, cancellationToken);
        }
        catch (AmazonS3Exception ex)
        {
            throw new StorageException(ex.Message, ex);
        }
    }
}