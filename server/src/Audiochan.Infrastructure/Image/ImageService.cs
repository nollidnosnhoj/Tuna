using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Audiochan.Infrastructure.Image
{
    public class ImageService : IImageService
    {
        private readonly IStorageService _storageService;

        public ImageService(IStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task<SaveBlobResponse> UploadImage(string data, string container, string blobName,
            CancellationToken cancellationToken = default)
        {
            // Parse the base64 data
            if (data.Contains("base64"))
                data = data.Split("base64")[1].Trim(',');

            var bytes = Convert.FromBase64String(data);

            // Resize the image to 500 x 500.
            using var imageContext = SixLabors.ImageSharp.Image.Load(bytes);
            var resizedImage = imageContext.Clone(x => x.Resize(500, 500));

            // Save the image context to JPEG
            var imageStream = new MemoryStream();
            await resizedImage.SaveAsJpegAsync(imageStream, cancellationToken);
            imageStream.Seek(0, SeekOrigin.Begin);

            return await _storageService.SaveAsync(
                stream: imageStream,
                container: container,
                blobName: blobName,
                metadata: null,
                cancellationToken: cancellationToken);
        }
    }
}