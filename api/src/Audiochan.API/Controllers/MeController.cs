using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Extensions;
using Audiochan.API.Models;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.GetAudioFeed;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Features.Auth.GetCurrentUser;
using Audiochan.Core.Features.FavoriteAudios.CheckIfAudioFavorited;
using Audiochan.Core.Features.FavoriteAudios.SetFavoriteAudio;
using Audiochan.Core.Features.FavoritePlaylists.CheckIfPlaylistFavorited;
using Audiochan.Core.Features.FavoritePlaylists.SetFavoritePlaylist;
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
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.API.Controllers
{
    [Authorize]
    [Route("me")]
    [ProducesResponseType(401)]
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
        [ProducesResponseType(200)]
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
        [ProducesResponseType(200)]
        [Produces("application/json")]
        [SwaggerOperation(
            Summary = "Returns information about authenticated user",
            Description = "Requires authentication.",
            OperationId = "GetYourInfo",
            Tags = new[] {"me"}
        )]
        public async Task<ActionResult<CurrentUserViewModel>> GetYourInfo(CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetCurrentUserQuery(), cancellationToken);
            return result != null
                ? Ok(result)
                : Unauthorized(ErrorApiResponse.Unauthorized("You are not authorized access."));
        }

        [HttpGet("feed", Name = "GetAuthenticatedUserFeed")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [SwaggerOperation(
            Summary = "Returns a list of tracks uploaded by authenticated user's followings.",
            Description = "Requires authentication.",
            OperationId = "GetYourFeed",
            Tags = new[] {"me"}
        )]
        public async Task<ActionResult<PagedListDto<AudioViewModel>>> GetYourFeed(
            [FromQuery] PaginationQueryParams queryParams, 
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
        [ProducesResponseType(200)]
        [SwaggerOperation(
            Summary = "Get authenticated user's uploaded audios.",
            Description = "Requires authentication",
            OperationId = "YourAudios",
            Tags = new [] {"me"}
        )]
        public async Task<ActionResult<GetAudioListViewModel>> GetYourAudios(
            [FromQuery] PaginationQueryParams queryParams, 
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

        [HttpHead("followings/{userId}", Name = "CheckIfUserFollowedUser")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [SwaggerOperation(
            Summary = "Check if the authenticated user follows a user",
            Description = "Requires authentication.",
            OperationId = "CheckIfYouFollowedUser",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> IsFollow(string userId, CancellationToken cancellationToken)
        {
            var request = new CheckIfUserIsFollowingQuery(_currentUserId, userId);
            return await _mediator.Send(request, cancellationToken)
                ? Ok()
                : NotFound();
        }

        [HttpPut("followings/{userId}", Name = "FollowUser")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [SwaggerOperation(
            Summary = "Follow a user",
            Description = "Requires authentication.",
            OperationId = "FollowUser",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> Follow(string userId, CancellationToken cancellationToken)
        {
            var request = new SetFollowCommand(_currentUserId, userId, true);
            var result = await _mediator.Send(request, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ReturnErrorResponse();
        }

        [HttpDelete("followings/{userId}", Name = "UnfollowUser")]
        [ProducesResponseType(204)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [SwaggerOperation(
            Summary = "Unfollow a user",
            Description = "Requires authentication.",
            OperationId = "UnfollowUser",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> Unfollow(string userId, CancellationToken cancellationToken)
        {
            var request = new SetFollowCommand(_currentUserId, userId, false);
            var result = await _mediator.Send(request, cancellationToken);
            return result.IsSuccess
                ? NoContent()
                : result.ReturnErrorResponse();
        }

        [HttpPut(Name = "UpdateUser")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [SwaggerOperation(
            Summary = "Updates authenticated user.",
            Description = "Requires authentication.",
            OperationId = "UpdateUser",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateProfileRequest request,
            CancellationToken cancellationToken)
        {
            var command = UpdateProfileCommand.FromRequest(_currentUserId, request);
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ReturnErrorResponse();
        }

        [HttpPatch("username", Name = "UpdateUsername")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(422)]
        [SwaggerOperation(
            Summary = "Updates authenticated user's username.",
            Description = "Requires authentication.",
            OperationId = "UpdateUsername",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> ChangeUsername([FromBody] UpdateUsernameRequest request,
            CancellationToken cancellationToken)
        {
            var command = UpdateUsernameCommand.FromRequest(_currentUserId, request);
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ReturnErrorResponse();
        }

        [HttpPatch("email", Name = "UpdateEmail")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(422)]
        [SwaggerOperation(
            Summary = "Updates authenticated user's email.",
            Description = "Requires authentication.",
            OperationId = "UpdateEmail",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> ChangeEmail([FromBody] UpdateEmailRequest request,
            CancellationToken cancellationToken)
        {
            var command = UpdateEmailCommand.FromRequest(_currentUserId, request);
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ReturnErrorResponse();
        }

        [HttpPatch("password", Name = "UpdatePassword")]
        [ProducesResponseType(200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(422)]
        [SwaggerOperation(
            Summary = "Updates authenticated user's password.",
            Description = "Requires authentication.",
            OperationId = "UpdatePassword",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> ChangePassword([FromBody] UpdatePasswordRequest request,
            CancellationToken cancellationToken)
        {
            var command = UpdatePasswordCommand.FromRequest(_currentUserId, request);
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ReturnErrorResponse();
        }

        [HttpPatch("picture")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [SwaggerOperation(
            Summary = "Add picture to user.",
            Description = "Requires authentication.",
            OperationId = "AddUserPicture",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> AddPicture([FromBody] ImageUploadRequest request,
            CancellationToken cancellationToken)
        {
            var command = UpdateUserPictureCommand.FromRequest(_currentUserId, request);
            var result = await _mediator.Send(command, cancellationToken);
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
            var query = new GetUserFavoriteAudiosQuery
            {
                Username = _currentUsername,
                Page = queryParams.Page,
                Size = queryParams.Size
            };
            var result = await _mediator.Send(query, cancellationToken);
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
            var command = new CheckIfAudioFavoritedQuery(audioId, _currentUserId);
            return await _mediator.Send(command, cancellationToken)
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
            var command = new SetFavoriteAudioCommand(audioId, _currentUserId, true);
            var result = await _mediator.Send(command, cancellationToken);
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
            var command = new SetFavoriteAudioCommand(audioId, _currentUserId, false);
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess
                ? NoContent()
                : result.ReturnErrorResponse();
        }

        [HttpHead("favorites/playlists/{playlistId:guid}", Name = "CheckIfUserFavoritedPlaylist")]
        [SwaggerOperation(
            Summary = "Check if the authenticated user favorited a playlist",
            Description = "Requires authentication.",
            OperationId = "CheckIfUserFavoritedPlaylist",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> IsFavoritePlaylist(Guid playlistId, CancellationToken cancellationToken)
        {
            var query = new CheckIfPlaylistFavoritedQuery(playlistId, _currentUserId);
            return await _mediator.Send(query, cancellationToken)
                ? Ok()
                : NotFound();
        }

        [HttpPut("favorites/playlists/{playlistId:guid}", Name = "FavoritePlaylist")]
        [SwaggerOperation(
            Summary = "Favorite a playlist",
            Description = "Requires authentication.",
            OperationId = "FavoritePlaylist",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> FavoritePlaylist(Guid playlistId, CancellationToken cancellationToken)
        {
            var command = new SetFavoritePlaylistCommand(playlistId, _currentUserId, true);
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok() : result.ReturnErrorResponse();
        }
        
        [HttpDelete("favorites/playlists/{playlistId:guid}", Name = "UnfavoritePlaylist")]
        [SwaggerOperation(
            Summary = "Unfavorite a playlist",
            Description = "Requires authentication.",
            OperationId = "UnfavoritePlaylist",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> UnfavoritePlaylist(Guid playlistId, CancellationToken cancellationToken)
        {
            var command = new SetFavoritePlaylistCommand(playlistId, _currentUserId, false);
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok() : result.ReturnErrorResponse();
        }
    }
}