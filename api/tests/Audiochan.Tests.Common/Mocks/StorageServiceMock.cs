using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Services;
using Audiochan.Core.Storage;

namespace Audiochan.Tests.Common.Mocks;

public class MockStorageService : IStorageService
{
    public bool ObjectDoesExist { get; }

    public MockStorageService(bool objectDoesExist)
    {
        ObjectDoesExist = objectDoesExist;
    }
        
    public string CreatePutPreSignedUrl(string bucket, string container, string blobName, int expirationInMinutes,
        Dictionary<string, string>? metadata = null)
    {
        return CreatePutPreSignedUrl(bucket, $"{container}/{blobName}", expirationInMinutes, metadata);
    }

    public string CreatePutPreSignedUrl(string bucket, string blobName, int expirationInMinutes, Dictionary<string, string>? metadata = null)
    {
        return "mock url";
    }

    public Task RemoveAsync(string bucket, string container, string blobName, CancellationToken cancellationToken = default)
    {
        return RemoveAsync(bucket, $"{container}/{blobName}", cancellationToken);
    }

    public Task RemoveAsync(string bucket, string blobName, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task SaveAsync(Stream stream, string bucket, string container, string blobName, Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        return SaveAsync(stream, bucket, $"{container}/{blobName}", metadata, cancellationToken);
    }

    public Task SaveAsync(Stream stream, string bucket, string blobName, Dictionary<string, string>? metadata = null,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<bool> ExistsAsync(string bucket, string container, string blobName, CancellationToken cancellationToken = default)
    {
        return ExistsAsync(bucket, $"{container}/{blobName}", cancellationToken);
    }

    public Task<bool> ExistsAsync(string bucket, string blobName, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(ObjectDoesExist);
    }

    public Task MoveBlobAsync(string sourceBucket, string sourceBlobName, string targetBucket, string? targetBlobName = null,
        CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}