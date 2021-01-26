using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

namespace Audiochan.Infrastructure.ImageSharp
{
    public class ImageService : IImageService
    {
        private readonly IStorageService _storageService;

        public ImageService(IStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task<BlobDto> UploadAudioImage(IFormFile file, string audioId,
            CancellationToken cancellationToken = default)
        {
            var container = Path.Combine(ContainerConstants.Artworks, ContainerConstants.Audios);
            return await ProcessAndUploadImage(file, container, audioId, cancellationToken);
        }

        public async Task<BlobDto> UploadUserImage(IFormFile file, string userId,
            CancellationToken cancellationToken = default)
        {
            var container = Path.Combine(ContainerConstants.Artworks, ContainerConstants.Users);
            return await ProcessAndUploadImage(file, container, userId, cancellationToken);
        }

        public async Task RemoveAudioImages(string audioId, CancellationToken cancellationToken = default)
        {
            var container = Path.Combine(ContainerConstants.Artworks, ContainerConstants.Audios);
            await DeleteArtworkAndThumbnails(container, audioId, cancellationToken);
        }

        public async Task RemoveUserImages(string userId, CancellationToken cancellationToken = default)
        {
            var container = Path.Combine(ContainerConstants.Artworks, ContainerConstants.Users);
            await DeleteArtworkAndThumbnails(container, userId, cancellationToken);
        }

        private async Task DeleteArtworkAndThumbnails(string container, string blobName,
            CancellationToken cancellationToken = default)
        {
            var task1 = _storageService.DeleteBlobAsync(
                container: container,
                blobName: blobName + "_large.jpg",
                cancellationToken);
            var task2 = _storageService.DeleteBlobAsync(
                container: container,
                blobName: blobName + "_medium.jpg",
                cancellationToken);
            var task3 = _storageService.DeleteBlobAsync(
                container: container,
                blobName: blobName + "small_.jpg",
                cancellationToken);
            await Task.WhenAll(task1, task2, task3);
        }
        
        private async Task<BlobDto> ProcessAndUploadImage(IFormFile file, string container, string blobName,
            CancellationToken cancellationToken = default)
        {
            using var image = await Image.LoadAsync(file.OpenReadStream());
            var imageOriginal = image.Clone(img => img.Resize(500, 500));
            var imageStream = new MemoryStream();
            await imageOriginal.SaveAsync(imageStream, new JpegEncoder(), cancellationToken);
            imageStream.Seek(0, SeekOrigin.Begin);
            var thumbnailMedium = await GenerateThumbnail(image, 200, 200, cancellationToken);
            var thumbnailSmall = await GenerateThumbnail(image, 100, 100, cancellationToken);
            var saveLargeImageIntoStorageTask = _storageService.SaveBlobAsync(
                container: container,
                blobName: blobName + "_large.jpg",
                stream: imageStream,
                overwrite: true,
                cancellationToken);
            var saveMediumImageIntoStorageTask = _storageService.SaveBlobAsync(
                container: container,
                blobName: blobName + "_medium.jpg",
                stream: thumbnailMedium.Stream,
                overwrite: true,
                cancellationToken);
            var saveSmallImageIntoStorageTask = _storageService.SaveBlobAsync(
                container: container,
                blobName: blobName + "_small.jpg",
                stream: thumbnailSmall.Stream,
                overwrite: true,
                cancellationToken);

            await Task.WhenAll(
                saveLargeImageIntoStorageTask, 
                saveMediumImageIntoStorageTask, 
                saveSmallImageIntoStorageTask);

            var blob = await _storageService.GetBlobAsync(
                container: container,
                blobName: blobName + "_large.jpg",
                cancellationToken);

            return blob;
        }
        
        private static async Task<ThumbnailDto> GenerateThumbnail(Image image, int height, int width, 
            CancellationToken cancellationToken = default)
        {
            var thumbnail = image.Clone(img => img.Resize(width, height));
            var thumbnailStream = new MemoryStream();
            await thumbnail.SaveAsync(thumbnailStream, new JpegEncoder(), cancellationToken);
            thumbnailStream.Seek(0, SeekOrigin.Begin);
            return new ThumbnailDto(thumbnailStream, height, width);
        }
    }
}