using Audiochan.Core.Extensions;
using Audiochan.Core.Settings;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Audiochan.Features.Auth.Register
{
    public class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidator(IOptions<IdentitySettings> options)
        {
            RuleFor(req => req.Username).Username(options.Value);
            RuleFor(req => req.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Email is invalid.");
            RuleFor(req => req.Password)
                .NotEmpty().WithMessage("Password is required.")
                .Password(options.Value);
        }
    }
}