using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons;
using Audiochan.Application.Commons.CQRS;
using Audiochan.Application.Commons.Services;
using Audiochan.Application.Commons.Extensions;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Application.Features.Upload.Commands.CreateUpload
{
    public record GenerateUploadLinkCommand : ICommandRequest<Result<GenerateUploadLinkResponse>>
    {
        public string FileName { get; init; } = null!;
        public long FileSize { get; init; }
    }

    public class GenerateUploadLinkCommandHandler 
        : IRequestHandler<GenerateUploadLinkCommand, Result<GenerateUploadLinkResponse>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IRandomIdGenerator _randomIdGenerator;
        private readonly IStorageService _storageService;
        private readonly AudioStorageSettings _audioStorageSettings;
        
        public GenerateUploadLinkCommandHandler(ICurrentUserService currentUserService,
            IRandomIdGenerator randomIdGenerator,
            IStorageService storageService,
            IOptions<MediaStorageSettings> mediaStorageSettings)
        {
            _currentUserService = currentUserService;
            _randomIdGenerator = randomIdGenerator;
            _storageService = storageService;
            _audioStorageSettings = mediaStorageSettings.Value.Audio;
        }
        
        public async Task<Result<GenerateUploadLinkResponse>> Handle(GenerateUploadLinkCommand command, 
            CancellationToken cancellationToken)
        {
            _currentUserService.User.TryGetUserId(out var userId);
            var (url, uploadId) = await CreateUploadUrl(command.FileName, userId);
            var response = new GenerateUploadLinkResponse { UploadId = uploadId, UploadUrl = url };
            return Result<GenerateUploadLinkResponse>.Success(response);
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