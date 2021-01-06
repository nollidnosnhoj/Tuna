using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Exceptions;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;

namespace Audiochan.Infrastructure.Storage
{
    public class FileStorageService : IStorageService
    {
        private readonly string _basePath;

        public FileStorageService(string basePath)
        {
            _basePath = basePath;
        }

        public async Task DeleteBlobAsync(string container, string blobName, CancellationToken cancellationToken = default)
        {
            await Task.Run(() => DeleteBlob(container, blobName), cancellationToken);
        }

        public async Task SaveBlobAsync(string container, string blobName, Stream stream, bool overwrite = true,
            CancellationToken cancellationToken = default)
        {
            var containerPath = Path.Combine(_basePath, container);

            var blobPath = Path.Combine(containerPath, blobName);

            if (!Path.GetFullPath(blobPath).StartsWith(containerPath, StringComparison.OrdinalIgnoreCase))
            {
                throw new StorageException("Wrong path.");
            }

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(blobPath) ?? containerPath);

                if (File.Exists(blobPath))
                    throw new StorageException("File already exists in file storage.");

                await using var file = File.Create(blobPath);
                await stream.CopyToAsync(file, cancellationToken);
            }
            catch (Exception ex)
            {
                throw new StorageException(ex.Message);
            }
        }

        public async Task<BlobDto> GetBlobAsync(string container, string blobName, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() => GetBlob(container, blobName), cancellationToken);
        }

        private void DeleteBlob(string container, string blobName)
        {
            var path = Path.Combine(_basePath, container, blobName);

            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception ex)
                {
                    throw new StorageException(ex.Message);
                }
            }
        }

        private BlobDto GetBlob(string container, string blobName)
        {
            var path = Path.Combine(_basePath, container, blobName);

            if (!File.Exists(path))
            {
                return new BlobDto(false, "", "", "", 0);
            }
            
            var file = new FileInfo(path);

            return new BlobDto(true, container, file.Name, $"{container}/{blobName}", file.Length);
        }
    }
}