using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Helpers;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Settings;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Features.Audios.GetUploadAudioUrl
{
    public class GetUploadAudioUrlRequestHandler : IRequestHandler<GetUploadAudioUrlRequest, GetUploadAudioUrlResponse>
    {
        private readonly MediaStorageSettings _storageSettings;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStorageService _storageService;

        public GetUploadAudioUrlRequestHandler(IStorageService storageService, ICurrentUserService currentUserService, IOptions<MediaStorageSettings> options)
        {
            _storageService = storageService;
            _currentUserService = currentUserService;
            _storageSettings = options.Value;
        }

        public Task<GetUploadAudioUrlResponse> Handle(GetUploadAudioUrlRequest request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId();
            var uploadId = UploadHelpers.GenerateUploadId();
            var blobName = uploadId + Path.GetExtension(request.FileName);
            var metadata = new Dictionary<string, string> {{"UserId", userId}, {"OriginalFilename", request.FileName}};
            var presignedUrl = _storageService.GetPresignedUrl(
                _storageSettings.Audio.Container, 
                blobName,
                5, 
                metadata);
            var response = new GetUploadAudioUrlResponse{Url = presignedUrl, UploadId = uploadId};
            return Task.FromResult(response);
        }
    }
}