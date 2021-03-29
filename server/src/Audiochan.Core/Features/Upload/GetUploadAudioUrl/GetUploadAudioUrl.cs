using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Options;
using Audiochan.Core.Interfaces;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Upload.GetUploadAudioUrl
{
    public record GetUploadAudioUrl : IRequest<GetUploadAudioUrlResponse>
    {
        public string FileName { get; init; }
        public long FileSize { get; init; }
    }

    public class GetUploadAudioUrlValidator : AbstractValidator<GetUploadAudioUrl>
    {
        public GetUploadAudioUrlValidator(IOptions<AudiochanOptions> options)
        {
            RuleFor(x => x.FileName)
                .NotEmpty()
                .WithMessage("Filename cannot be empty.")
                .Must(Path.HasExtension)
                .WithMessage("Filename must have a file extension")
                .Must(value => options.Value.AudioStorageOptions.ContentTypes.Contains(value.GetContentType()))
                .WithMessage("The file name's extension is invalid.");
            RuleFor(x => x.FileSize)
                .NotEmpty()
                .WithMessage("Filesize cannot be empty.")
                .Must(value => options.Value.AudioStorageOptions.MaxFileSize >= value)
                .WithMessage("Filesize exceeds maximum limit.");
        }
    }
    
    public class GetUploadAudioUrlHandler : IRequestHandler<GetUploadAudioUrl, GetUploadAudioUrlResponse>
    {
        private readonly AudiochanOptions _audiochanOptions;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStorageService _storageService;

        public GetUploadAudioUrlHandler(IStorageService storageService, ICurrentUserService currentUserService, IOptions<AudiochanOptions> options)
        {
            _storageService = storageService;
            _currentUserService = currentUserService;
            _audiochanOptions = options.Value;
        }

        public Task<GetUploadAudioUrlResponse> Handle(GetUploadAudioUrl request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.GetUserId();
            var uploadId = UploadHelpers.GenerateUploadId();
            var blobName = uploadId + Path.GetExtension(request.FileName);
            var metadata = new Dictionary<string, string> {{"UserId", userId}, {"OriginalFilename", request.FileName}};
            var presignedUrl = _storageService.GetPresignedUrl(
                _audiochanOptions.AudioStorageOptions.Container, 
                blobName,
                5, 
                metadata);
            var response = new GetUploadAudioUrlResponse{Url = presignedUrl, UploadId = uploadId};
            return Task.FromResult(response);
        }
    }
}