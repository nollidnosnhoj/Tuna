using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.CreateAudioUploadUrl
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
        private readonly MediaStorageSettings _storageSettings;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStorageService _storageService;
        private readonly INanoidGenerator _nanoid;
        
        public CreateAudioUploadUrlCommandHandler(IOptions<MediaStorageSettings> storageSettings, 
            ICurrentUserService currentUserService, 
            IStorageService storageService, INanoidGenerator nanoid)
        {
            _storageSettings = storageSettings.Value;
            _currentUserService = currentUserService;
            _storageService = storageService;
            _nanoid = nanoid;
        }
        
        public async Task<Result<CreateAudioUploadUrlResponse>> Handle(CreateAudioUploadUrlCommand command, 
            CancellationToken cancellationToken)
        {
            if (!_currentUserService.TryGetUserId(out var userId))
                return Result<CreateAudioUploadUrlResponse>.Unauthorized();
            
            var fileExt = Path.GetExtension(command.FileName);
            var uploadId = await CreateRandomBlobNameForUpload();
            var blobName = uploadId + fileExt;
            var url = GetUploadUrl(blobName, userId);
            var response = new CreateAudioUploadUrlResponse {UploadUrl = url, UploadId = uploadId};
            return Result<CreateAudioUploadUrlResponse>.Success(response);
        }

        private async Task<string> CreateRandomBlobNameForUpload()
        {
            return await _nanoid.GenerateAsync(size: 21);
        }

        private string GetUploadUrl(string blobName, long ownerId)
        {
            var metadata = new Dictionary<string, string> {{"UserId", ownerId.ToString()}};
            return _storageService.CreatePutPresignedUrl(
                _storageSettings.Audio.TempBucket,
                blobName,
                5,
                metadata);
        }
    }
}