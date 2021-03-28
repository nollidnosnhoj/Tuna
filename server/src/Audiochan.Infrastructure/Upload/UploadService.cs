using System.Collections.Generic;
using System.IO;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Options;
using Audiochan.Core.Interfaces;
using Microsoft.Extensions.Options;

namespace Audiochan.Infrastructure.Upload
{
    public class UploadService : IUploadService
    {
        private readonly AudiochanOptions.StorageOptions _audioStorageOptions;
        private readonly IStorageService _storageService;
        private readonly ICurrentUserService _currentUserService;

        public UploadService(IOptions<AudiochanOptions> options,
            IStorageService storageService,
            ICurrentUserService currentUserService)
        {
            _audioStorageOptions = options.Value.AudioStorageOptions;
            _storageService = storageService;
            _currentUserService = currentUserService;
        }

        public GetUploadAudioUrlResponse GetUploadAudioUrl(GetUploadAudioUrlRequest request)
        {
            var userId = _currentUserService.GetUserId();
            var uploadId = UploadHelpers.GenerateUploadId();
            var blobName = uploadId + Path.GetExtension(request.FileName);
            var metadata = new Dictionary<string, string> {{"UserId", userId}, {"OriginalFilename", request.FileName}};
            var presignedUrlResponse = _storageService.GetPresignedUrl(
                _audioStorageOptions.Container, 
                blobName,
                5, 
                metadata);
            return new GetUploadAudioUrlResponse{Url = presignedUrlResponse.Url, UploadId = uploadId};
        }
    }
}