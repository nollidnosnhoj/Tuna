using System.IO;
using System.Threading;
using System.Threading.Tasks;
using ATL;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Audiochan.Infrastructure.AudioUpload
{
    public class AudioUploadService : IAudioUploadService
    {
        private readonly IStorageService _storageService;

        public AudioUploadService(IStorageService storageService)
        {
            _storageService = storageService;
        }

        public async Task<AudioUploadResult> UploadAudio(IFormFile audioFile, string id, CancellationToken cancellationToken = default)
        {
            await using (var audioStream = audioFile.OpenReadStream())
            {
                var audioMetadata = new Track(audioStream, audioFile.ContentType);
                var blobName = id + Path.GetExtension(audioFile.FileName);
                await _storageService
                    .SaveBlobAsync(ContainerConstants.Audios, blobName, audioStream, false, cancellationToken);
                var blobDto = await _storageService
                    .GetBlobAsync(ContainerConstants.Audios, blobName, cancellationToken);

                return new AudioUploadResult(
                    blobDto.Url,
                    audioMetadata.Duration,
                    blobDto.Size,
                    Path.GetExtension(audioFile.FileName));
            }
        }

        public async Task DeleteAudio(string id, string fileExt, CancellationToken cancellationToken = default)
        {
            await _storageService.DeleteBlobAsync(ContainerConstants.Audios, id + fileExt, cancellationToken);
        }
    }
}