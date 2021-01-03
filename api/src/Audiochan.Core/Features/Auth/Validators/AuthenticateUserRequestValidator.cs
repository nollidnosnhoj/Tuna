using Audiochan.Core.Features.Auth.Models;
using FluentValidation;

namespace Audiochan.Core.Features.Auth.Validators
{
    public class AuthenticateUserRequestValidator : AbstractValidator<LoginRequest>
    {
        public AuthenticateUserRequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}