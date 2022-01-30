using Audiochan.Application.Commons.Extensions;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Audiochan.Application.Features.Users.Commands.UpdateUsername
{
    public class UpdateUsernameCommandValidator : AbstractValidator<UpdateUsernameCommand>
    {
        public UpdateUsernameCommandValidator(IOptions<IdentitySettings> options)
        {
            RuleFor(req => req.NewUsername).UsernameValidation(options.Value.UsernameSettings);
        }
    }
}