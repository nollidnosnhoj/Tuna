using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Settings;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Audiochan.Infrastructure.Shared
{
    internal class ImageProcessingService : IImageProcessingService
    {
        private readonly IStorageService _storageService;
        private readonly MediaStorageSettings _storageSettings;

        public ImageProcessingService(IStorageService storageService, IOptions<MediaStorageSettings> storageSettings)
        {
            _storageService = storageService;
            _storageSettings = storageSettings.Value;
        }

        public async Task UploadImage(string data, string container, string blobName,
            CancellationToken cancellationToken = default)
        {
            var bytes = FromBase64StringToBytes(data);

            using var imageContext = SixLabors.ImageSharp.Image.Load(bytes);
            var resizedImage = ModifyImage(imageContext);
            var imageStream = await SaveImageAsJpeg(resizedImage, cancellationToken);

            await _storageService.SaveAsync(
                stream: imageStream,
                bucket: _storageSettings.Audio.Bucket,
                container: container,
                blobName: blobName,
                metadata: null,
                cancellationToken: cancellationToken);
        }

        private static async Task<MemoryStream> SaveImageAsJpeg(SixLabors.ImageSharp.Image imageContext,
            CancellationToken cancellationToken)
        {
            // Save the image context to JPEG
            var imageStream = new MemoryStream();
            await imageContext.SaveAsJpegAsync(imageStream, cancellationToken);
            imageStream.Seek(0, SeekOrigin.Begin);
            return imageStream;
        }

        private static Image<Rgba32> ModifyImage(Image<Rgba32> imageContext)
        {
            // Resize to 500x500
            var resizedImage = imageContext.Clone(x => x.Resize(500, 500));
            return resizedImage;
        }

        private static byte[] FromBase64StringToBytes(string data)
        {
            // Parse the base64 data
            if (data.Contains("base64"))
                data = data.Split("base64")[1].Trim(',');

            var bytes = Convert.FromBase64String(data);
            return bytes;
        }
    }
}