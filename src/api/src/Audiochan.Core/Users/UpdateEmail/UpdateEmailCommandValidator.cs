using FluentValidation;

namespace Audiochan.Core.Users
{
    public class UpdateEmailCommandValidator : AbstractValidator<UpdateEmailCommand>
    {
        public UpdateEmailCommandValidator()
        {
            RuleFor(req => req.NewEmail)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email is invalid.");
        }
    }
}