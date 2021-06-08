using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Extensions;
using Audiochan.API.Models;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.GetAudioFeed;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Features.Auth.GetCurrentUser;
using Audiochan.Core.Features.FavoriteAudios.CheckIfFavoriting;
using Audiochan.Core.Features.FavoriteAudios.SetFavorite;
using Audiochan.Core.Features.Followers.CheckIfFollowing;
using Audiochan.Core.Features.Followers.SetFollow;
using Audiochan.Core.Features.Users.UpdateEmail;
using Audiochan.Core.Features.Users.UpdatePassword;
using Audiochan.Core.Features.Users.UpdatePicture;
using Audiochan.Core.Features.Users.UpdateProfile;
using Audiochan.Core.Features.Users.UpdateUsername;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class MeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly string _currentUserId;

        public MeController(ICurrentUserService currentUserService, IMediator mediator)
        {
            _mediator = mediator;
            _currentUserId = currentUserService.GetUserId();
        }

        [HttpHead(Name = "IsAuthenticated")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerOperation(
            Summary = "Check if authenticated",
            Description = "Requires authentication.",
            OperationId = "IsAuthenticated",
            Tags = new[] {"me"}
        )]
        public IActionResult IsAuthenticated()
        {
            return Ok();
        }

        [HttpGet(Name = "GetAuthenticatedUser")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CurrentUserViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Returns information about authenticated user",
            Description = "Requires authentication.",
            OperationId = "GetAuthenticatedUser",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> GetAuthenticatedUser(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCurrentUserQuery(), cancellationToken);
            return result != null
                ? Ok(result)
                : Unauthorized(ErrorApiResponse.Unauthorized("You are not authorized access."));
        }

        [HttpGet("feed", Name = "GetAuthenticatedUserFeed")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedListDto<AudioViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(
            Summary = "Returns a list of tracks uploaded by authenticated user's followings.",
            Description = "Requires authentication.",
            OperationId = "GetAuthenticatedUserFeed",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> GetAuthenticatedUserFeed(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAudioFeedQuery
            {
                UserId = _currentUserId
            }, cancellationToken);
            return Ok(result);
        }

        [HttpHead("followings/{username}", Name = "CheckIfUserFollowedUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Check if the authenticated user follows a user",
            Description = "Requires authentication.",
            OperationId = "CheckIfUserFollowedUser",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> IsFollow(string username, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new CheckIfUserIsFollowingQuery(_currentUserId, username), cancellationToken)
                ? Ok()
                : NotFound();
        }

        [HttpPut("followings/{username}", Name = "FollowUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [SwaggerOperation(
            Summary = "Follow a user",
            Description = "Requires authentication.",
            OperationId = "FollowUser",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> Follow(string username, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new SetFollowCommand(_currentUserId, username, true), cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ReturnErrorResponse();
        }

        [HttpDelete("followings/{username}", Name = "UnfollowUser")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [SwaggerOperation(
            Summary = "Unfollow a user",
            Description = "Requires authentication.",
            OperationId = "UnfollowUser",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> Unfollow(string username, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new SetFollowCommand(_currentUserId, username, false), cancellationToken);
            return result.IsSuccess
                ? NoContent()
                : result.ReturnErrorResponse();
        }

        [HttpPut(Name = "UpdateUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [SwaggerOperation(
            Summary = "Updates authenticated user.",
            Description = "Requires authentication.",
            OperationId = "UpdateUser",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateProfileCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with {UserId = _currentUserId}, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ReturnErrorResponse();
        }

        [HttpPatch("username", Name = "UpdateUsername")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [SwaggerOperation(
            Summary = "Updates authenticated user's username.",
            Description = "Requires authentication.",
            OperationId = "UpdateUsername",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> ChangeUsername([FromBody] UpdateUsernameCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with {UserId = _currentUserId}, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ReturnErrorResponse();
        }

        [HttpPatch("email", Name = "UpdateEmail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [SwaggerOperation(
            Summary = "Updates authenticated user's email.",
            Description = "Requires authentication.",
            OperationId = "UpdateEmail",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> ChangeEmail([FromBody] UpdateEmailCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with {UserId = _currentUserId}, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ReturnErrorResponse();
        }

        [HttpPatch("password", Name = "UpdatePassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        [SwaggerOperation(
            Summary = "Updates authenticated user's password.",
            Description = "Requires authentication.",
            OperationId = "UpdatePassword",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> ChangePassword([FromBody] UpdatePasswordCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command with {UserId = _currentUserId}, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ReturnErrorResponse();
        }

        [HttpPatch("picture")]
        [SwaggerOperation(
            Summary = "Add picture to user.",
            Description = "Requires authentication.",
            OperationId = "AddUserPicture",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> AddPicture([FromBody] UpdateUserPictureCommand command,
            CancellationToken cancellationToken)
        {
            command.UserId = _currentUserId;
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess
                ? Ok(result.Data)
                : result.ReturnErrorResponse();
        }

        [HttpHead("favorites/audio/{audioId:guid}", Name="CheckIfUserFavoritedAudio")]
        [SwaggerOperation(
            Summary = "Check if the authenticated user favorited an audio",
            Description = "Requires authentication.",
            OperationId = "CheckIfUserFavoritedAudio",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> IsFavoriteAudio(Guid audioId, CancellationToken cancellationToken)
        {
            return await _mediator.Send(new CheckIfUserFavoritedAudioQuery(audioId, _currentUserId),
                cancellationToken)
                ? Ok()
                : NotFound();
        }
        
        [HttpPut("favorites/audio/{audioId:guid}", Name = "FavoriteAudio")]
        [SwaggerOperation(
            Summary = "Favorite an audio",
            Description = "Requires authentication.",
            OperationId = "FavoriteAudio",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> FavoriteAudio(Guid audioId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new SetFavoriteAudioCommand(audioId, _currentUserId, true), 
                cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ReturnErrorResponse();
        }

        [HttpDelete("favorites/audio/{audioId:guid}", Name = "UnfavoriteAudio")]
        [SwaggerOperation(
            Summary = "Unfavorite an audio",
            Description = "Requires authentication.",
            OperationId = "UnfavoriteAudio",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> UnfavoriteAudio(Guid audioId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new SetFavoriteAudioCommand(audioId, _currentUserId, false), 
                cancellationToken);
            return result.IsSuccess
                ? NoContent()
                : result.ReturnErrorResponse();
        }
    }
}