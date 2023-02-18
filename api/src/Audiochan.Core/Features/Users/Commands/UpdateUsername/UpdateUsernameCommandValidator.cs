using Audiochan.Common.Extensions;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Users.Commands.UpdateUsername
{
    public class UpdateUsernameCommandValidator : AbstractValidator<UpdateUsernameCommand>
    {
        public UpdateUsernameCommandValidator()
        {
            RuleFor(req => req.NewUserName).UsernameValidation();
        }
    }
}