using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Settings;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.CreateAudioUploadUrl
{
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
}