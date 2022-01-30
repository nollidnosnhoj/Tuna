using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Services;

namespace Audiochan.Infrastructure.Storage.File
{
    public class FileStorageService : IStorageService
    {
        private readonly string _basePath;

        public FileStorageService(string basePath)
        {
            _basePath = basePath;
        }
        
        public string CreatePutPreSignedUrl(string bucket, string container, string blobName, int expirationInMinutes,
            Dictionary<string, string>? metadata = null)
        {
            throw new NotImplementedException();
        }

        public string CreatePutPreSignedUrl(string bucket, string blobName, int expirationInMinutes, Dictionary<string, string>? metadata = null)
        {
            throw new NotImplementedException();
        }

        public Task RemoveAsync(string bucket, string container, string blobName, CancellationToken cancellationToken = default)
        {
            container = container.Replace('/', Path.PathSeparator);
            blobName = Path.Combine(container, blobName);
            return RemoveAsync(bucket, blobName, cancellationToken);
        }

        public Task RemoveAsync(string bucket, string blobName, CancellationToken cancellationToken = default)
        {
            var path = Path.Combine(_basePath, bucket, blobName);
            if (!System.IO.File.Exists(path)) throw new StorageException("File does not exist");
            System.IO.File.Delete(path);
            return Task.CompletedTask;
        }

        public async Task SaveAsync(Stream stream, string bucket, string container, string blobName, Dictionary<string, string>? metadata = null,
            CancellationToken cancellationToken = default)
        {
            container = container.Replace('/', Path.PathSeparator);
            blobName = Path.Combine(container, blobName);
            await SaveAsync(stream, bucket, blobName, metadata, cancellationToken);
        }

        public async Task SaveAsync(Stream stream, string bucket, string blobName, Dictionary<string, string>? metadata = null,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var path = Path.Combine(_basePath, bucket, blobName);
                Directory.CreateDirectory(Path.GetDirectoryName(path)!);
                await using (var file = System.IO.File.Create(path))
                {
                    await stream.CopyToAsync(file, cancellationToken);
                }

                await stream.DisposeAsync();
            }
            catch (Exception ex)
            {
                throw new StorageException(ex.Message);
            }
        }

        public Task<bool> ExistsAsync(string bucket, string container, string blobName, CancellationToken cancellationToken = default)
        {
            container = container.Replace('/', Path.PathSeparator);
            blobName = Path.Combine(container, blobName);
            return ExistsAsync(bucket, blobName, cancellationToken);
        }

        public Task<bool> ExistsAsync(string bucket, string blobName, CancellationToken cancellationToken = default)
        {
            var path = Path.Combine(_basePath, bucket, blobName);
            return Task.FromResult(System.IO.File.Exists(path));
        }

        public Task MoveBlobAsync(string sourceBucket, string sourceBlobName, string targetBucket, string? targetBlobName = null,
            CancellationToken cancellationToken = default)
        {
            var sourcePath = Path.Combine(_basePath, sourceBucket, sourceBlobName);
            var targetPath = Path.Combine(_basePath, targetBucket, targetBlobName ?? sourceBlobName);

            if (!System.IO.File.Exists(sourcePath)) throw new StorageException("File does not exists.");
            
            System.IO.File.Move(sourcePath, targetPath);

            return Task.CompletedTask;
        }
    }
}