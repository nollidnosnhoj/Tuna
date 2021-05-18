using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Features.Users.UpdateUser
{
    public record UpdateUserDetailsRequest : IRequest<IResult<bool>>
    {
        [JsonIgnore] public string UserId { get; init; }
        public string DisplayName { get; init; }
        public string About { get; init; }
        public string Website { get; init; }
    }

    public class UpdateUserDetailsRequestHandler : IRequestHandler<UpdateUserDetailsRequest, IResult<bool>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public UpdateUserDetailsRequestHandler(UserManager<User> userManger, ICurrentUserService currentUserService)
        {
            _userManager = userManger;
            _currentUserService = currentUserService;
        }

        public async Task<IResult<bool>> Handle(UpdateUserDetailsRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return Result<bool>.Fail(ResultError.NotFound);
            if (user.Id != _currentUserService.GetUserId())
                return Result<bool>.Fail(ResultError.Forbidden);

            user.UpdateDisplayName(request.DisplayName);
            user.UpdateAbout(request.About);
            user.UpdateWebsite(request.Website);

            await _userManager.UpdateAsync(user);
            return Result<bool>.Success(true);
        }
    }
}