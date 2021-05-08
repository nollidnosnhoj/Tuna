using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Builders;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Settings;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.UploadAudio
{
    public record UploadAudioRequest : IRequest<UploadAudioResponse>
    {
        public string FileName { get; init; }
        public long FileSize { get; init; }
        public decimal Duration { get; init; }
    }
    
    public class UploadAudioRequestHandler : IRequestHandler<UploadAudioRequest, UploadAudioResponse>
    {
        private readonly MediaStorageSettings _storageSettings;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStorageService _storageService;
        private readonly IApplicationDbContext _dbContext;
        
        public UploadAudioRequestHandler(IOptions<MediaStorageSettings> storageSettings, 
            ICurrentUserService currentUserService, 
            IStorageService storageService, 
            IApplicationDbContext dbContext)
        {
            _storageSettings = storageSettings.Value;
            _currentUserService = currentUserService;
            _storageService = storageService;
            _dbContext = dbContext;
        }
        
        public async Task<UploadAudioResponse> Handle(UploadAudioRequest request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId();
            var fileExt = Path.GetExtension(request.FileName);
            var title = Path.GetFileNameWithoutExtension(request.FileName).Truncate(30);

            var audioBuilder = new AudioBuilder()
                .AddUserId(userId)
                .AddFileName(request.FileName)
                .AddFileExtension(fileExt)
                .AddDuration(request.Duration)
                .AddFileSize(request.FileSize)
                .AddTitle(title);

            var audio = await audioBuilder.BuildAsync();

            await _dbContext.Audios.AddAsync(audio, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            var metadata = new Dictionary<string, string> {{"UserId", userId}, {"OriginalFilename", request.FileName}};
            var presignedUrl = _storageService.GetPresignedUrl(
                method: "put",
                bucket: _storageSettings.Audio.Bucket,
                container: _storageSettings.Audio.Container,
                blobName: audio.Id + fileExt,
                expirationInMinutes: 5,
                metadata: metadata);
            return new UploadAudioResponse {UploadUrl = presignedUrl, AudioId = audio.Id};
        }
    }
}