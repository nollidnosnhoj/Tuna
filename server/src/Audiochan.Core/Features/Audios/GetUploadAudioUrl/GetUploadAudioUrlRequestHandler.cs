using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Settings;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.GetUploadAudioUrl
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
                method: "put",
                bucket: _storageSettings.Audio.Bucket,
                container: _storageSettings.Audio.Container, 
                blobName: blobName,
                expirationInMinutes: 5, 
                metadata: metadata);
            var response = new GetUploadAudioUrlResponse{Url = presignedUrl, UploadId = uploadId};
            return Task.FromResult(response);
        }
    }
}