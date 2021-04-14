using Audiochan.Core.Extensions;
using Audiochan.Core.Settings;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Audiochan.API.Features.Upload.GetUploadAudioUrl
{
    public class GetUploadAudioUrlRequestValidator : AbstractValidator<GetUploadAudioUrlRequest>
    {
        public GetUploadAudioUrlRequestValidator(IOptions<MediaStorageSettings> options)
        {
            RuleFor(x => x.FileName)
                .FileNameValidation(options.Value.Audio.ValidContentTypes);
            RuleFor(x => x.FileSize)
                .FileSizeValidation(options.Value.Audio.MaximumFileSize);
        }
    }
}