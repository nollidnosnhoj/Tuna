using Audiochan.Application.Commons.Extensions;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Audiochan.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserCommandValidator(IOptions<IdentitySettings> identitySettings)
    {
        When(x => !string.IsNullOrEmpty(x.Username), () =>
        {
            RuleFor(x => x.Username)!
                .UsernameValidation(identitySettings.Value.UsernameSettings);
        });

        When(x => !string.IsNullOrEmpty(x.Email), () =>
        {
            RuleFor(x => x.Email)!
                .NotEmpty().EmailAddress();
        });
    }
}