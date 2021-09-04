using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Audios.CreateAudioUploadUrl
{
    public record CreateAudioUploadUrlCommand : IRequest<Result<CreateAudioUploadUrlResponse>>
    {
        public string FileName { get; init; } = null!;
        public long FileSize { get; init; }
    }
    
    public class CreateAudioUploadUrlCommandValidator : AbstractValidator<CreateAudioUploadUrlCommand>
    {
        public CreateAudioUploadUrlCommandValidator(IOptions<MediaStorageSettings> options)
        {
            var uploadOptions = options.Value.Audio;
            RuleFor(req => req.FileSize)
                .FileSizeValidation(uploadOptions.MaximumFileSize);
            RuleFor(req => req.FileName)
                .FileNameValidation(uploadOptions.ValidContentTypes);
        }
    }
    
    public class CreateAudioUploadUrlCommandHandler 
        : IRequestHandler<CreateAudioUploadUrlCommand, Result<CreateAudioUploadUrlResponse>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IRandomIdGenerator _randomIdGenerator;
        private readonly IStorageService _storageService;
        private readonly AudioStorageSettings _audioStorageSettings;
        
        public CreateAudioUploadUrlCommandHandler(ICurrentUserService currentUserService,
            IRandomIdGenerator randomIdGenerator,
            IStorageService storageService,
            IOptions<MediaStorageSettings> mediaStorageSettings)
        {
            _currentUserService = currentUserService;
            _randomIdGenerator = randomIdGenerator;
            _storageService = storageService;
            _audioStorageSettings = mediaStorageSettings.Value.Audio;
        }
        
        public async Task<Result<CreateAudioUploadUrlResponse>> Handle(CreateAudioUploadUrlCommand command, 
            CancellationToken cancellationToken)
        {
            if (!_currentUserService.TryGetUserId(out var userId))
                return Result<CreateAudioUploadUrlResponse>.Unauthorized();

            var (url, uploadId) = await CreateUploadUrl(command.FileName, userId);
            var response = new CreateAudioUploadUrlResponse { UploadId = uploadId, UploadUrl = url };
            return Result<CreateAudioUploadUrlResponse>.Success(response);
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