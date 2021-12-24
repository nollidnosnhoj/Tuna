using Audiochan.Application.Commons.Extensions;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Audiochan.Application.Features.Users.Commands.UpdatePassword
{
    public class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommand>
    {
        public UpdatePasswordCommandValidator(IOptions<IdentitySettings> options)
        {
            RuleFor(req => req.NewPassword)
                .NotEmpty()
                .WithMessage("New Password is required.")
                .NotEqual(req => req.CurrentPassword)
                .WithMessage("New password cannot be the same as the previous.")
                .PasswordValidation(options.Value.PasswordSettings, "New Password");
        }
    }
}