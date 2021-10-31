using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Attributes;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Audios.Commands
{
    [Authorize]
    public record CreateUploadCommand : IRequest<Result<UploadResponse>>
    {
        public string FileName { get; init; } = null!;
        public long FileSize { get; init; }
    }

    public class CreateUploadCommandHandler 
        : IRequestHandler<CreateUploadCommand, Result<UploadResponse>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IRandomIdGenerator _randomIdGenerator;
        private readonly IStorageService _storageService;
        private readonly AudioStorageSettings _audioStorageSettings;
        
        public CreateUploadCommandHandler(ICurrentUserService currentUserService,
            IRandomIdGenerator randomIdGenerator,
            IStorageService storageService,
            IOptions<MediaStorageSettings> mediaStorageSettings)
        {
            _currentUserService = currentUserService;
            _randomIdGenerator = randomIdGenerator;
            _storageService = storageService;
            _audioStorageSettings = mediaStorageSettings.Value.Audio;
        }
        
        public async Task<Result<UploadResponse>> Handle(CreateUploadCommand command, 
            CancellationToken cancellationToken)
        {
            _currentUserService.User.TryGetUserId(out var userId);
            var (url, uploadId) = await CreateUploadUrl(command.FileName, userId);
            var response = new UploadResponse { UploadId = uploadId, UploadUrl = url };
            return Result<UploadResponse>.Success(response);
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