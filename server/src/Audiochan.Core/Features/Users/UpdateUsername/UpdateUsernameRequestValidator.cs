using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Options;
using FluentValidation;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Users.UpdateUsername
{
    public class UpdateUsernameRequestValidator : AbstractValidator<UpdateUsernameRequest>
    {
        public UpdateUsernameRequestValidator(IOptions<IdentityOptions> options)
        {
            RuleFor(req => req.NewUsername).Username(options.Value);
        }
    }
}