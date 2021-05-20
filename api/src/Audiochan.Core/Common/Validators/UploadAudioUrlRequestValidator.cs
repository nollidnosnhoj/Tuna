using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Settings;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Common.Validators
{
    public class UploadAudioUrlRequestValidator : AbstractValidator<UploadAudioUrlRequest>
    {
        public UploadAudioUrlRequestValidator(IOptions<MediaStorageSettings> options)
        {
            RuleFor(x => x.FileName)
                .FileNameValidation(options.Value.Audio.ValidContentTypes);
            RuleFor(x => x.FileSize)
                .FileSizeValidation(options.Value.Audio.MaximumFileSize);
        }
    }
}