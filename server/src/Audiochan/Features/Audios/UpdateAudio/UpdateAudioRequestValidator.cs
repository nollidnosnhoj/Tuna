using Audiochan.Core.Validators;
using FluentValidation;

namespace Audiochan.Features.Audios.UpdateAudio
{
    public class UpdateAudioRequestValidator : AbstractValidator<UpdateAudioRequest>
    {
        public UpdateAudioRequestValidator()
        {
            Include(new AudioAbstractRequestValidator());
        }
    }
}