using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Features.Users.Models;
using FluentValidation;

namespace Audiochan.Core.Features.Users.Validators
{
    public class UpdateUsernameRequestValidator : AbstractValidator<UpdateUsernameRequest>
    {
        public UpdateUsernameRequestValidator()
        {
            RuleFor(req => req.Username).Username();
        }
    }
}