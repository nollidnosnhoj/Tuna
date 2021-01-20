using Audiochan.Core.Features.Auth.Models;
using FluentValidation;

namespace Audiochan.Core.Features.Auth.Validators
{
    public class LoginRequestRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestRequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}