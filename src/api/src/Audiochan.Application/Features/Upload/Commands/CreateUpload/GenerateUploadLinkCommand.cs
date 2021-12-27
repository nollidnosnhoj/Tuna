using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Services;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Application.Features.Upload.Commands.CreateUpload
{
    public record GenerateUploadLinkCommand(string FileName, long FileSize)
        : ICommandRequest<GenerateUploadLinkResponse>;

    public class GenerateUploadLinkCommandHandler 
        : IRequestHandler<GenerateUploadLinkCommand, GenerateUploadLinkResponse>
    {
        private readonly IRandomIdGenerator _randomIdGenerator;
        private readonly IStorageService _storageService;
        private readonly AudioStorageSettings _audioStorageSettings;
        private readonly ICurrentUserService _currentUserService;
        
        public GenerateUploadLinkCommandHandler(IRandomIdGenerator randomIdGenerator,
            IStorageService storageService,
            IOptions<MediaStorageSettings> mediaStorageSettings, ICurrentUserService currentUserService)
        {
            _randomIdGenerator = randomIdGenerator;
            _storageService = storageService;
            _currentUserService = currentUserService;
            _audioStorageSettings = mediaStorageSettings.Value.Audio;
        }
        
        public async Task<GenerateUploadLinkResponse> Handle(GenerateUploadLinkCommand command, 
            CancellationToken cancellationToken)
        {
            var userId = _currentUserService.User.GetUserId();
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