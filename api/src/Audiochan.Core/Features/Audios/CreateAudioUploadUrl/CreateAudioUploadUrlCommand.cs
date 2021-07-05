using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.CreateAudioUploadUrl
{
    public record CreateAudioUploadUrlCommand : IRequest<CreateAudioUploadUrlResponse>
    {
        public string FileName { get; init; } = null!;
        public long FileSize { get; init; }
    }
    
    public class CreateAudioUploadUrlCommandHandler : IRequestHandler<CreateAudioUploadUrlCommand, CreateAudioUploadUrlResponse>
    {
        private readonly MediaStorageSettings _storageSettings;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStorageService _storageService;
        private readonly INanoidGenerator _nanoid;
        
        public CreateAudioUploadUrlCommandHandler(IOptions<MediaStorageSettings> storageSettings, 
            ICurrentUserService currentUserService, 
            IStorageService storageService, INanoidGenerator nanoid)
        {
            _storageSettings = storageSettings.Value;
            _currentUserService = currentUserService;
            _storageService = storageService;
            _nanoid = nanoid;
        }
        
        public async Task<CreateAudioUploadUrlResponse> Handle(CreateAudioUploadUrlCommand command, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId();
            var fileExt = Path.GetExtension(command.FileName);
            var objectId = await _nanoid.GenerateAsync(size: 21);
            var blobName = objectId + fileExt;

            var metadata = new Dictionary<string, string> {{"UserId", userId}};
            var presignedUrl = _storageService.CreatePutPresignedUrl(
                _storageSettings.Audio.TempBucket,
                _storageSettings.Audio.Container,
                blobName,
                5,
                metadata);
            return new CreateAudioUploadUrlResponse {UploadUrl = presignedUrl, UploadId = objectId};
        }
    }
}