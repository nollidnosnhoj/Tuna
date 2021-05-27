using Audiochan.API.Features.Shared.Requests;
using FluentValidation;

namespace Audiochan.API.Features.Shared.Validators
{
    public class AudioAbstractRequestValidator : AbstractValidator<AudioAbstractRequest>
    {
        public AudioAbstractRequestValidator()
        {
            RuleFor(req => req.Title)
                .NotEmpty()
                .WithMessage("Title is required.")
                .MaximumLength(30)
                .WithMessage("Title cannot be no more than 30 characters long.");

            RuleFor(req => req.Description)
                .MaximumLength(500)
                .WithMessage("Description cannot be more than 500 characters long.");

            RuleFor(req => req.Tags)
                .Must(u => u!.Count <= 10)
                .WithMessage("Can only have up to 10 tags per audio upload.");
        }
    }
}