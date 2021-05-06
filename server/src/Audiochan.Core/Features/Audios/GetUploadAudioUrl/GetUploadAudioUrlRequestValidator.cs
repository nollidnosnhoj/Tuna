using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Settings;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.GetUploadAudioUrl
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