using FluentValidation;

namespace Audiochan.Features.Auth.Login
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.Login).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}