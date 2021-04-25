using Audiochan.Core.Extensions;
using Audiochan.Core.Models.Settings;
using Audiochan.Core.Validators;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Audiochan.Features.Audios.CreateAudio
{
    public class CreateAudioRequestValidator : AbstractValidator<CreateAudioRequest>
    {
        public CreateAudioRequestValidator(IOptions<MediaStorageSettings> options)
        {
            var uploadOptions = options.Value.Audio;

            RuleFor(req => req.UploadId)
                .NotEmpty()
                .WithMessage("UploadId is required.");
            RuleFor(req => req.Duration)
                .NotEmpty()
                .WithMessage("Duration is required.");
            RuleFor(req => req.FileSize)
                .FileSizeValidation(uploadOptions.MaximumFileSize);
            RuleFor(req => req.FileName)
                .FileNameValidation(uploadOptions.ValidContentTypes);

            Include(new AudioAbstractRequestValidator());
        }
    }
}