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
            
            When(req => !string.IsNullOrWhiteSpace(req.Title), () =>
            {
                RuleFor(req => req.Title)
                    .MaximumLength(30)
                    .WithMessage("Title cannot be no more than 30 characters long.");
            });
            
            RuleFor(req => req.Description)
                .MaximumLength(500).WithMessage("Description cannot be more than 500 characters long.");

            When(req => req.Tags?.Count > 0, () =>
            {
                RuleFor(req => req.Tags)
                    .Must(u => u!.Count <= 10)
                    .WithMessage("Can only have up to 10 tags per audio upload.");
            });
        }
    }
}