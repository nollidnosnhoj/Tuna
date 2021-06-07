using Audiochan.API.Extensions;
using Audiochan.Core.Extensions;
using Audiochan.Core.Settings;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Audiochan.API.Features.Audios.UploadAudio
{
    public class UploadAudioRequestValidator : AbstractValidator<UploadAudioRequest>
    {
        public UploadAudioRequestValidator(IOptions<MediaStorageSettings> options)
        {
            var uploadOptions = options.Value.Audio;
            RuleFor(req => req.FileSize)
                .FileSizeValidation(uploadOptions.MaximumFileSize);
            RuleFor(req => req.FileName)
                .FileNameValidation(uploadOptions.ValidContentTypes);
        }
    }
}