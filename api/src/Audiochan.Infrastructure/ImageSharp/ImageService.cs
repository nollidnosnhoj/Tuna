using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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

        private async Task<BlobDto> UploadArtwork(Image image, string container, string name,
            CancellationToken cancellationToken = default)
        {
            var imageStream = new MemoryStream();
            var imageOriginal = image.Clone(img => img.Resize(500, 500));
            await imageOriginal.SaveAsync(imageStream, new JpegEncoder(), cancellationToken);
            imageStream.Position = 0;
            var thumbnailMd = GenerateThumbnail(image, 200, 200);
            var thumbnailSm = GenerateThumbnail(image, 100, 100);
            var task1 = _storageService.SaveBlobAsync(
                container: container,
                blobName: name + "-large.jpg",
                stream: imageStream,
                overwrite: true,
                cancellationToken);
            var task2 = _storageService.SaveBlobAsync(
                container: container,
                blobName: name + "-medium.jpg",
                stream: thumbnailMd.Stream,
                overwrite: true,
                cancellationToken);
            var task3 = _storageService.SaveBlobAsync(
                container: container,
                blobName: name + "-small.jpg",
                stream: thumbnailSm.Stream,
                overwrite: true,
                cancellationToken);

            await Task.WhenAll(task1, task2, task3);

            var blob = await _storageService.GetBlobAsync(
                container: container,
                blobName: name + "-large.jpg",
                cancellationToken);

            return blob;
        }
        
        private ThumbnailDto GenerateThumbnail(Image image, int height, int width)
        {
            var thumbnail = image.Clone(img => img.Resize(width, height));
            var thumbnailStream = new MemoryStream();
            thumbnail.Save(thumbnailStream, new JpegEncoder());
            thumbnailStream.Position = 0;
            return new ThumbnailDto(thumbnailStream, height, width);
        }

        public async Task<BlobDto> UploadArtwork(string imageData, string container, string name,
            CancellationToken cancellationToken = default)
        {
            var bytes = Convert.FromBase64String(imageData);
            using var image = Image.Load(bytes);
            return await UploadArtwork(image, container, name, cancellationToken);
        }

        public async Task<BlobDto> UploadArtwork(IFormFile file, string container, string name,
            CancellationToken cancellationToken = default)
        {
            using var image = await Image.LoadAsync(file.OpenReadStream());
            return await UploadArtwork(image, container, name, cancellationToken);
        }

        public async Task DeleteArtworkAndThumbnails(string container, string name, CancellationToken cancellationToken = default)
        {
            var task1 = _storageService.DeleteBlobAsync(
                container: container,
                blobName: name + "-large.jpg",
                cancellationToken);
            var task2 = _storageService.DeleteBlobAsync(
                container: container,
                blobName: name + "-medium.jpg",
                cancellationToken);
            var task3 = _storageService.DeleteBlobAsync(
                container: container,
                blobName: name + "-small.jpg",
                cancellationToken);
            await Task.WhenAll(task1, task2, task3);
        }
    }
}