using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Options;
using Audiochan.Core.Features.Users.Models;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Users.Validators
{
    public class UpdateUsernameRequestValidator : AbstractValidator<UpdateUsernameRequest>
    {
        public UpdateUsernameRequestValidator(IOptions<IdentityUserOptions> options)
        {
            RuleFor(req => req.Username).Username(options.Value);
        }
    }
}