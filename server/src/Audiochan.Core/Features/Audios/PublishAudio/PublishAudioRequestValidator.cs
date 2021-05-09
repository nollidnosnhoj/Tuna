using Audiochan.Core.Common.Validators;
using FluentValidation;

namespace Audiochan.Core.Features.Audios.PublishAudio
{
    public class PublishAudioRequestValidator : AbstractValidator<PublishAudioRequest>
    {
        public PublishAudioRequestValidator()
        {
            Include(new AudioAbstractRequestValidator());
        }
    }
}