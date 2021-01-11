using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Features.Audios.Models;
using FluentValidation;

namespace Audiochan.Core.Features.Audios.Validators
{
    public class UploadAudioRequestValidator : AbstractValidator<UploadAudioRequest>
    {
        private readonly string[] _validContentTypes =
        {
            "audio/mpeg",
            "audio/x-mpeg",
            "audio/mp3",
            "audio/x-mp3",
            "audio/mpeg3",
            "audio/x-mpeg3",
            "audio/mpg",
            "audio/x-mpg",
            "audio/x-mpegaudio"
        };
        
        public UploadAudioRequestValidator()
        {
            RuleFor(req => req.File)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("File is required.")
                .FileValidation(_validContentTypes, 2000000 * 1000);
            
            Include(new UpdateAudioRequestValidator());
        }
    }
}