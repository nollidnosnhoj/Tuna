using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Helpers;
using Audiochan.Core;
using Audiochan.Core.Services;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Audiochan.Infrastructure.Shared
{
    internal class ImageService : IImageService
    {
        private readonly IStorageService _storageService;
        private readonly PictureStorageSettings _storageSettings;

        public ImageService(IStorageService storageService, IOptions<MediaStorageSettings> mediaStorageOptions)
        {
            _storageService = storageService;
            var mediaStorageSettings = mediaStorageOptions.Value;
            _storageSettings = mediaStorageSettings.Image;
        }

        public async Task UploadImage(string data, string container, string blobName,
            CancellationToken cancellationToken = default)
        {
            var bytes = EncodingHelpers.ConvertBase64ToBytes(data);

            using var imageContext = Image.Load(bytes);
            var resizedImage = ModifyImage(imageContext);
            var imageStream = await SaveImageAsJpeg(resizedImage, cancellationToken);

            await _storageService.SaveAsync(
                stream: imageStream,
                bucket: _storageSettings.Bucket,
                container: container,
                blobName: blobName,
                metadata: null,
                cancellationToken: cancellationToken);
        }

        public async Task RemoveImage(string container, string blobName, CancellationToken cancellationToken = default)
        {
            await _storageService.RemoveAsync(_storageSettings.Bucket, container, blobName, cancellationToken);
        }

        public bool ValidateImageSize(string base64, int min, int max, int? minHeight = null, int? maxHeight = null)
        {
            var info = GetImageInfoFromBase64(base64);
            return info.Width >= min
                   && info.Width <= max
                   && info.Height >= (minHeight ?? min)
                   && info.Height <= (maxHeight ?? max);
        }

        private static IImageInfo GetImageInfoFromBase64(string base64)
        {
            var bytes = EncodingHelpers.ConvertBase64ToBytes(base64);
            var imageInfo = Image.Identify(bytes);
            if (imageInfo is null) throw new Exception("Image Info detector not suitable for image.");
            return imageInfo;
        }

        private static async Task<MemoryStream> SaveImageAsJpeg(Image imageContext,
            CancellationToken cancellationToken)
        {
            // Save the image context to JPEG
            var imageStream = new MemoryStream();
            await imageContext.SaveAsJpegAsync(imageStream, cancellationToken);
            imageStream.Seek(0, SeekOrigin.Begin);
            return imageStream;
        }

        private static Image ModifyImage(Image imageContext)
        {
            // Resize to 500x500
            var resizedImage = imageContext.Clone(x => x.Resize(500, 500));
            return resizedImage;
        }
    }
}