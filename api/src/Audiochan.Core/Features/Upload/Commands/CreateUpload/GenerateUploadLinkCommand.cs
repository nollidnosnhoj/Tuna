using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Mediatr;
using Audiochan.Common.Dtos;
using Audiochan.Common.Extensions;
using Audiochan.Common.Services;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Upload.Commands.CreateUpload
{
    public class GenerateUploadLinkCommand : AuthCommandRequest<GenerateUploadLinkResponse>
    {
        public string FileName { get; init; }
        public long FileSize { get; init; }

        public GenerateUploadLinkCommand(string fileName, long fileSize, ClaimsPrincipal user) : base(user)
        {
            FileName = fileName;
            FileSize = fileSize;
        }
    }

    public class GenerateUploadLinkCommandHandler 
        : IRequestHandler<GenerateUploadLinkCommand, GenerateUploadLinkResponse>
    {
        private readonly IRandomIdGenerator _randomIdGenerator;
        private readonly IStorageService _storageService;
        private readonly AudioStorageSettings _audioStorageSettings;
        
        public GenerateUploadLinkCommandHandler(
            IRandomIdGenerator randomIdGenerator,
            IStorageService storageService,
            IOptions<MediaStorageSettings> mediaStorageSettings)
        {
            _randomIdGenerator = randomIdGenerator;
            _storageService = storageService;
            _audioStorageSettings = mediaStorageSettings.Value.Audio;
        }
        
        public async Task<GenerateUploadLinkResponse> Handle(GenerateUploadLinkCommand command, 
            CancellationToken cancellationToken)
        {
            var userId = command.GetUserId();
            var (url, uploadId) = await CreateUploadUrl(command.FileName, userId);
            var response = new GenerateUploadLinkResponse { UploadId = uploadId, UploadUrl = url };
            return response;
        }
        
        private async Task<(string, string)> CreateUploadUrl(string fileName, long userId)
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
    }
}