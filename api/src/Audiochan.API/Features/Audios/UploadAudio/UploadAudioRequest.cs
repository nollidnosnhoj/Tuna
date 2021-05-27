using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Services;
using Audiochan.Core.Settings;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.API.Features.Audios.UploadAudio
{
    public record UploadAudioRequest : IRequest<UploadAudioResponse>
    {
        public string FileName { get; init; } = null!;
        public long FileSize { get; init; }
    }
    
    public class UploadAudioRequestHandler : IRequestHandler<UploadAudioRequest, UploadAudioResponse>
    {
        private readonly MediaStorageSettings _storageSettings;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStorageService _storageService;
        
        public UploadAudioRequestHandler(IOptions<MediaStorageSettings> storageSettings, 
            ICurrentUserService currentUserService, 
            IStorageService storageService)
        {
            _storageSettings = storageSettings.Value;
            _currentUserService = currentUserService;
            _storageService = storageService;
        }
        
        public async Task<UploadAudioResponse> Handle(UploadAudioRequest request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId();
            var fileExt = Path.GetExtension(request.FileName);
            var objectId = await Nanoid.Nanoid.GenerateAsync();
            var blobName = objectId + fileExt;

            var metadata = new Dictionary<string, string> {{"UserId", userId}, {"OriginalFilename", request.FileName}};
            var presignedUrl = _storageService.CreatePutPresignedUrl(
                _storageSettings.Audio.TempBucket,
                _storageSettings.Audio.Container,
                blobName,
                5,
                metadata);
            return new UploadAudioResponse {UploadUrl = presignedUrl, UploadId = blobName};
        }
    }
}