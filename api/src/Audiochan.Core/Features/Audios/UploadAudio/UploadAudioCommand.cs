﻿using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.UploadAudio
{
    public record UploadAudioCommand : IRequest<UploadAudioResponse>
    {
        public string FileName { get; init; } = null!;
        public long FileSize { get; init; }
    }
    
    public class UploadAudioCommandHandler : IRequestHandler<UploadAudioCommand, UploadAudioResponse>
    {
        private readonly MediaStorageSettings _storageSettings;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStorageService _storageService;
        
        public UploadAudioCommandHandler(IOptions<MediaStorageSettings> storageSettings, 
            ICurrentUserService currentUserService, 
            IStorageService storageService)
        {
            _storageSettings = storageSettings.Value;
            _currentUserService = currentUserService;
            _storageService = storageService;
        }
        
        public async Task<UploadAudioResponse> Handle(UploadAudioCommand command, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId();
            var fileExt = Path.GetExtension(command.FileName);
            var objectId = await Nanoid.Nanoid.GenerateAsync();
            var blobName = objectId + fileExt;

            var metadata = new Dictionary<string, string> {{"UserId", userId}};
            var presignedUrl = _storageService.CreatePutPresignedUrl(
                _storageSettings.Audio.TempBucket,
                _storageSettings.Audio.Container,
                blobName,
                5,
                metadata);
            return new UploadAudioResponse {UploadUrl = presignedUrl, UploadId = objectId};
        }
    }
}