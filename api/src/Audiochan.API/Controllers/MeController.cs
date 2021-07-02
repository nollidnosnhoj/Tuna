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
using Audiochan.Core.Features.Users.GetUserAudios;
using Audiochan.Core.Features.Users.GetUserFavoriteAudios;
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
        private readonly string _currentUsername;

        public MeController(ICurrentUserService currentUserService, IMediator mediator)
        {
            _mediator = mediator;
            _currentUserId = currentUserService.GetUserId();
            _currentUsername = currentUserService.GetUsername();
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
            OperationId = "GetYourInfo",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> GetYourInfo(CancellationToken cancellationToken)
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
            OperationId = "GetYourFeed",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> GetYourFeed([FromQuery] PaginationQueryParams queryParams, 
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAudioFeedQuery
            {
                UserId = _currentUserId,
                Page = queryParams.Page,
                Size = queryParams.Size
            }, cancellationToken);
            return Ok(result);
        }

        [HttpGet("audios")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(GetAudioListViewModel), StatusCodes.Status200OK)]
        [SwaggerOperation(
            Summary = "Get authenticated user's uploaded audios.",
            Description = "Requires authentication",
            OperationId = "YourAudios",
            Tags = new [] {"me"}
        )]
        public async Task<IActionResult> GetYourAudios([FromQuery] PaginationQueryParams queryParams, 
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetUsersAudioQuery
            {
                Page = queryParams.Page,
                Size = queryParams.Size,
                Username = _currentUsername
            }, cancellationToken);
            return Ok(result);
        }

        [HttpHead("followings/{username}", Name = "CheckIfUserFollowedUser")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Check if the authenticated user follows a user",
            Description = "Requires authentication.",
            OperationId = "CheckIfYouFollowedUser",
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
        public async Task<IActionResult> UpdateUser([FromBody] UpdateProfileRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(UpdateProfileCommand.FromRequest(_currentUserId, request), cancellationToken);
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
        public async Task<IActionResult> ChangeUsername([FromBody] UpdateUsernameRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(UpdateUsernameCommand.FromRequest(_currentUserId, request), cancellationToken);
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
        public async Task<IActionResult> ChangeEmail([FromBody] UpdateEmailRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(UpdateEmailCommand.FromRequest(_currentUserId, request), cancellationToken);
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
        public async Task<IActionResult> ChangePassword([FromBody] UpdatePasswordRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(UpdatePasswordCommand.FromRequest(_currentUserId, request), cancellationToken);
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
        public async Task<IActionResult> AddPicture([FromBody] ImageDataDto request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(UpdateUserPictureCommand.FromRequest(_currentUserId, request), cancellationToken);
            return result.IsSuccess
                ? Ok(result.Data)
                : result.ReturnErrorResponse();
        }
        
        [HttpGet("favorite/audios")]
        [SwaggerOperation(
            Summary = "Get Your favorite audios",
            Description = "Requires authentication.",
            OperationId = "YourFavoriteAudios",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> GetYourFavoriteAudios([FromQuery] PaginationQueryParams queryParams,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetUserFavoriteAudiosQuery
            {
                Username = _currentUsername,
                Page = queryParams.Page,
                Size = queryParams.Size
            }, cancellationToken);
            return Ok(result);
        }

        [HttpHead("favorites/audios/{audioId:guid}", Name="CheckIfUserFavoritedAudio")]
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
        
        [HttpPut("favorites/audios/{audioId:guid}", Name = "FavoriteAudio")]
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

        [HttpDelete("favorites/audios/{audioId:guid}", Name = "UnfavoriteAudio")]
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