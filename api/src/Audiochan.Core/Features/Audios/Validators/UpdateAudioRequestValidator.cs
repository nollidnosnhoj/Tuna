using Audiochan.Core.Features.Audios.Models;
using FluentValidation;

namespace Audiochan.Core.Features.Audios.Validators
{
    public class UpdateAudioRequestValidator : AbstractValidator<UpdateAudioRequest>
    {
        public UpdateAudioRequestValidator()
        {
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