using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Services
{
    public interface IAudioUploadService
    {
        Task<(string, string)> CreateUploadUrl(string fileName, long userId);
        Task MoveTempAudioToPublic(string fileName, CancellationToken cancellationToken = default);
        Task<bool> ExistsInTempStorage(string fileName, CancellationToken cancellationToken = default);
        Task RemoveAudioFromStorage(string fileName, CancellationToken cancellationToken = default);
    }
    
    public class AudioUploadService : IAudioUploadService
    {
        private readonly IRandomIdGenerator _randomIdGenerator;
        private readonly IStorageService _storageService;
        private readonly AudioStorageSettings _audioStorageSettings;
        

        public AudioUploadService(IRandomIdGenerator randomIdGenerator, 
            IStorageService storageService,
            IOptions<MediaStorageSettings> mediaStorageSettings)
        {
            _randomIdGenerator = randomIdGenerator;
            _storageService = storageService;
            _audioStorageSettings = mediaStorageSettings.Value.Audio;
        }

        public async Task<(string, string)> CreateUploadUrl(string fileName, long userId)
        {
            var fileExt = Path.GetExtension(fileName);
            var uploadId = await _randomIdGenerator.GenerateAsync(size: 21);
            var blobName = uploadId + fileExt;
            var metadata = new Dictionary<string, string> {{"UserId", userId.ToString()}};
            var url = _storageService.CreatePutPreSignedUrl(
                _audioStorageSettings.TempBucket,
                blobName,
                5,
                metadata);
            return (url, uploadId);
        }

        public async Task MoveTempAudioToPublic(string fileName, CancellationToken cancellationToken = default)
        {
            await _storageService.MoveBlobAsync(
                _audioStorageSettings.TempBucket,
                fileName,
                _audioStorageSettings.Bucket,
                $"audios/{fileName}",
                cancellationToken: cancellationToken);
        }
        
        public async Task<bool> ExistsInTempStorage(string fileName, CancellationToken cancellationToken = default)
        {
            return await _storageService.ExistsAsync(
                _audioStorageSettings.TempBucket,
                fileName,
                cancellationToken);
        }

        public async Task RemoveAudioFromStorage(string fileName, CancellationToken cancellationToken = default)
        {
            await _storageService.RemoveAsync(
                _audioStorageSettings.Bucket,
                $"audios/{fileName}",
                cancellationToken);
        }
    }
}