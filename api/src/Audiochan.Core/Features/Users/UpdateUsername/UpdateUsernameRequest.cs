﻿using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Features.Users.UpdateUsername
{
    public record UpdateUsernameRequest : IRequest<IResult<bool>>
    {
        [JsonIgnore] public string? UserId { get; init; }
        public string NewUsername { get; init; } = null!;
    }

    public class UpdateUsernameRequestHandler : IRequestHandler<UpdateUsernameRequest, IResult<bool>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public UpdateUsernameRequestHandler(UserManager<User> userManger, ICurrentUserService currentUserService)
        {
            _userManager = userManger;
            _currentUserService = currentUserService;
        }

        public async Task<IResult<bool>> Handle(UpdateUsernameRequest request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null) return Result<bool>.Fail(ResultError.Unauthorized);
            if (user.Id != _currentUserService.GetUserId())
                return Result<bool>.Fail(ResultError.Forbidden);

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