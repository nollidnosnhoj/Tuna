using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Services;
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
        private readonly ICurrentUserService _currentUserService;
        private readonly IAudioUploadService _audioUploadService;
        
        public CreateAudioUploadUrlCommandHandler(ICurrentUserService currentUserService, 
            IAudioUploadService audioUploadService)
        {
            _currentUserService = currentUserService;
            _audioUploadService = audioUploadService;
        }
        
        public async Task<Result<CreateAudioUploadUrlResponse>> Handle(CreateAudioUploadUrlCommand command, 
            CancellationToken cancellationToken)
        {
            if (!_currentUserService.TryGetUserId(out var userId))
                return Result<CreateAudioUploadUrlResponse>.Unauthorized();

            var (url, uploadId) = await _audioUploadService.CreateUploadUrl(command.FileName, userId);
            var response = new CreateAudioUploadUrlResponse { UploadId = uploadId, UploadUrl = url };
            return Result<CreateAudioUploadUrlResponse>.Success(response);
        }
    }
}