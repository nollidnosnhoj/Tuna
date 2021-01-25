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
            var container = Path.Combine(ContainerConstants.Artworks, ContainerConstants.Audios, audioId);
            return await ProcessAndUploadImage(file, container, cancellationToken);
        }

        public async Task<BlobDto> UploadUserImage(IFormFile file, string userId,
            CancellationToken cancellationToken = default)
        {
            var container = Path.Combine(ContainerConstants.Artworks, ContainerConstants.Users, userId);
            return await ProcessAndUploadImage(file, container, cancellationToken);
        }

        public async Task RemoveAudioImages(string audioId, CancellationToken cancellationToken = default)
        {
            var container = Path.Combine(ContainerConstants.Artworks, ContainerConstants.Audios, audioId);
            await DeleteArtworkAndThumbnails(container, cancellationToken);
        }

        public async Task RemoveUserImages(string userId, CancellationToken cancellationToken = default)
        {
            var container = Path.Combine(ContainerConstants.Artworks, ContainerConstants.Users, userId);
            await DeleteArtworkAndThumbnails(container, cancellationToken);
        }

        private async Task DeleteArtworkAndThumbnails(string container,
            CancellationToken cancellationToken = default)
        {
            var task1 = _storageService.DeleteBlobAsync(
                container: container,
                blobName: "large.jpg",
                cancellationToken);
            var task2 = _storageService.DeleteBlobAsync(
                container: container,
                blobName: "medium.jpg",
                cancellationToken);
            var task3 = _storageService.DeleteBlobAsync(
                container: container,
                blobName: "small.jpg",
                cancellationToken);
            await Task.WhenAll(task1, task2, task3);
        }
        
        private async Task<BlobDto> ProcessAndUploadImage(IFormFile file, string container,
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
                blobName: "large.jpg",
                stream: imageStream,
                overwrite: true,
                cancellationToken);
            var saveMediumImageIntoStorageTask = _storageService.SaveBlobAsync(
                container: container,
                blobName: "medium.jpg",
                stream: thumbnailMedium.Stream,
                overwrite: true,
                cancellationToken);
            var saveSmallImageIntoStorageTask = _storageService.SaveBlobAsync(
                container: container,
                blobName: "small.jpg",
                stream: thumbnailSmall.Stream,
                overwrite: true,
                cancellationToken);

            await Task.WhenAll(
                saveLargeImageIntoStorageTask, 
                saveMediumImageIntoStorageTask, 
                saveSmallImageIntoStorageTask);

            var blob = await _storageService.GetBlobAsync(
                container: container,
                blobName: "large.jpg",
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