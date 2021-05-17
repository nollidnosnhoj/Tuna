using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Common.Validators;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.CreateAudio
{
    public class CreateAudioRequestValidator : AbstractValidator<CreateAudioRequest>
    {
        public CreateAudioRequestValidator(IOptions<MediaStorageSettings> options)
        {
            var uploadOptions = options.Value.Audio;
            RuleFor(req => req.Duration)
                .NotEmpty()
                .WithMessage("Duration is required.");
            RuleFor(req => req.FileSize)
                .FileSizeValidation(uploadOptions.MaximumFileSize);
            RuleFor(req => req.FileName)
                .FileNameValidation(uploadOptions.ValidContentTypes);
            RuleFor(req => req.ContentType)
                .NotEmpty()
                .WithMessage("Content Type is required.")
                .Must((type) => uploadOptions.ValidContentTypes.Contains(type))
                .WithMessage("Content Type is invalid.");
            Include(new AudioAbstractRequestValidator());
        }
    }
}