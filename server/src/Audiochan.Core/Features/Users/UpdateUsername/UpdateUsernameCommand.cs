using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Entities;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using IdentityOptions = Audiochan.Core.Common.Options.IdentityOptions;

namespace Audiochan.Core.Features.Users.UpdateUsername
{
    public record UpdateUsernameCommand : IRequest<IResult<bool>>
    {
        [JsonIgnore] public string UserId { get; init; }
        public string NewUsername { get; init; }
    }

    public class UpdateUsernameCommandValidator : AbstractValidator<UpdateUsernameCommand>
    {
        public UpdateUsernameCommandValidator(IOptions<IdentityOptions> options)
        {
            RuleFor(req => req.NewUsername).Username(options.Value);
        }
    }

    public class UpdateUsernameCommandHandler : IRequestHandler<UpdateUsernameCommand, IResult<bool>>
    {
        private readonly UserManager<User> _userManager;

        public UpdateUsernameCommandHandler(UserManager<User> userManger)
        {
            _userManager = userManger;
        }

        public async Task<IResult<bool>> Handle(UpdateUsernameCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return Result<bool>.Fail(ResultError.Unauthorized);
            // update username
            var result = await _userManager.SetUserNameAsync(user, request.NewUsername);
            if (!result.Succeeded) result.ToResult();
            // update normalized username
            await _userManager.UpdateNormalizedUserNameAsync(user);
            // update display name
            user.DisplayName = user.UserName;
            await _userManager.UpdateAsync(user);
            return Result<bool>.Success(true);
        }
    }
}