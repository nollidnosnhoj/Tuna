using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Services;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.CreateAudio
{
    public class CreateAudioCommandValidator : AbstractValidator<CreateAudioCommand>
    {
        private readonly MediaStorageSettings _storageSettings;
        private readonly IStorageService _storageService;
        
        public CreateAudioCommandValidator(IOptions<MediaStorageSettings> options, IStorageService storageService)
        {
            _storageService = storageService;
            _storageSettings = options.Value;
            RuleFor(req => req.UploadId)
                .NotEmpty()
                .WithMessage("UploadId is required.");
            RuleFor(req => req.BlobName)
                .MustAsync(ExistsInTempStorageAsync)
                .WithMessage("Cannot find uploaded file.");
            RuleFor(req => req.Duration)
                .NotEmpty()
                .WithMessage("Duration is required.");
            RuleFor(req => req.FileSize)
                .FileSizeValidation(_storageSettings.Audio.MaximumFileSize);
            RuleFor(req => req.FileName)
                .FileNameValidation(_storageSettings.Audio.ValidContentTypes);
            RuleFor(req => req.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MaximumLength(30)
                .WithMessage("Title cannot be no more than 30 characters long.");
            RuleFor(req => req.Description)
                .NotNull()
                .WithMessage("Description cannot be null.")
                .MaximumLength(500)
                .WithMessage("Description cannot be more than 500 characters long.");
            RuleFor(req => req.Tags)
                .NotNull()
                .WithMessage("Tags cannot be null.")
                .Must(u => u!.Count <= 10)
                .WithMessage("Can only have up to 10 tags per audio upload.");
        }
        
        private async Task<bool> ExistsInTempStorageAsync(string blobName, CancellationToken cancellationToken = default)
        {
            return await _storageService.ExistsAsync(
                _storageSettings.Audio.TempBucket,
                _storageSettings.Audio.Container,
                blobName,
                cancellationToken);
        }
    }
}