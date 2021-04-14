using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Enums;
using Audiochan.Core.Models.Interfaces;
using Audiochan.Core.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Features.Users.UpdateUser
{
    public class UpdateUserDetailsRequestHandler : IRequestHandler<UpdateUserDetailsRequest, IResult<bool>>
    {
        private readonly UserManager<User> _userManager;

        public UpdateUserDetailsRequestHandler(UserManager<User> userManger)
        {
            _userManager = userManger;
        }

        public async Task<IResult<bool>> Handle(UpdateUserDetailsRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return Result<bool>.Fail(ResultError.Unauthorized);

            user.UpdateDisplayName(request.DisplayName);
            user.UpdateAbout(request.About);
            user.UpdateWebsite(request.Website);

            await _userManager.UpdateAsync(user);
            return Result<bool>.Success(true);
        }
    }
}