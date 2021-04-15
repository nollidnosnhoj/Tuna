using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Enums;
using Audiochan.Core.Extensions;
using Audiochan.Core.Models.Interfaces;
using Audiochan.Core.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Features.Users.UpdateUsername
{
    public class UpdateUsernameRequestHandler : IRequestHandler<UpdateUsernameRequest, IResult<bool>>
    {
        private readonly UserManager<User> _userManager;

        public UpdateUsernameRequestHandler(UserManager<User> userManger)
        {
            _userManager = userManger;
        }

        public async Task<IResult<bool>> Handle(UpdateUsernameRequest request, CancellationToken cancellationToken)
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