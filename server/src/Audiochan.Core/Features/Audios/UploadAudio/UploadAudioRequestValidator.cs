using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Settings;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.UploadAudio
{
    public class UploadAudioRequestValidator : AbstractValidator<UploadAudioRequest>
    {
        public UploadAudioRequestValidator(IOptions<MediaStorageSettings> options)
        {
            var uploadOptions = options.Value.Audio;
            RuleFor(req => req.Duration)
                .NotEmpty()
                .WithMessage("Duration is required.");
            RuleFor(req => req.FileSize)
                .FileSizeValidation(uploadOptions.MaximumFileSize);
            RuleFor(req => req.FileName)
                .FileNameValidation(uploadOptions.ValidContentTypes);
        }
    }
}