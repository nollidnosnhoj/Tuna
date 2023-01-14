﻿using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Extensions;
using Audiochan.Common.Extensions;
using Audiochan.Core.Features.Users.Commands.SetFollow;
using Audiochan.Core.Features.Users.Queries.CheckIfFollowing;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.API.Controllers.Me
{
    [Area("me")]
    [Authorize]
    [Route("[area]/followings/{userId:long}")]
    [ProducesResponseType(401)]
    public class FollowingsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly long _currentUserId;

        public FollowingsController(ICurrentUserService currentUserService, IMediator mediator)
        {
            _mediator = mediator;
            currentUserService.User.TryGetUserId(out _currentUserId);
        }
        
        [HttpHead(Name = "CheckIfUserFollowedUser")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [SwaggerOperation(
            Summary = "Check if the authenticated user follows a user",
            Description = "Requires authentication.",
            OperationId = "CheckIfYouFollowedUser",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> IsFollow(long userId, CancellationToken cancellationToken)
        {
            var request = new CheckIfUserIsFollowingQuery(_currentUserId, userId);
            return await _mediator.Send(request, cancellationToken)
                ? Ok()
                : NotFound();
        }

        [HttpPut(Name = "FollowUser")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [SwaggerOperation(
            Summary = "Follow a user",
            Description = "Requires authentication.",
            OperationId = "FollowUser",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> Follow(long userId, CancellationToken cancellationToken)
        {
            var request = new SetFollowCommand(_currentUserId, userId, true);
            var result = await _mediator.Send(request, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ReturnErrorResponse();
        }

        [HttpDelete(Name = "UnfollowUser")]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [SwaggerOperation(
            Summary = "Unfollow a user",
            Description = "Requires authentication.",
            OperationId = "UnfollowUser",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> Unfollow(long userId, CancellationToken cancellationToken)
        {
            var request = new SetFollowCommand(_currentUserId, userId, false);
            var result = await _mediator.Send(request, cancellationToken);
            return result.IsSuccess
                ? NoContent()
                : result.ReturnErrorResponse();
        }
    }
}