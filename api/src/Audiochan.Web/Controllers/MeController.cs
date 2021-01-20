using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Features.Users.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Audiochan.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class MeController : ControllerBase
    {
        private readonly IAudioService _audioService;
        private readonly IUserService _userService;
        private readonly IFollowerService _followerService;
        private readonly IFavoriteService _favoriteService;
        private readonly long _currentUserId;
        private readonly string _currentUsername;

        public MeController(ICurrentUserService currentUserService, IAudioService audioService, 
            IUserService userService, IFollowerService followerService, IFavoriteService favoriteService)
        {
            _audioService = audioService;
            _userService = userService;
            _followerService = followerService;
            _favoriteService = favoriteService;
            _currentUserId = currentUserService.GetUserId();
            _currentUsername = currentUserService.GetUsername();
        }

        [HttpHead(Name="IsAuthenticated")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult CheckAuth()
        {
            return Ok();
        }

        [HttpGet(Name="GetAuthenticatedUser")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(CurrentUserViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAuthenticatedUser(CancellationToken cancellationToken)
        {
            var result = await _userService.GetCurrentUser(_currentUserId, cancellationToken);
            
            return result.IsSuccess 
                ? Ok(result.Data) 
                : result.ReturnErrorResponse();
        }

        [HttpGet("feed", Name="GetAudioFeed")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<AudioListViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAuthenticatedUserFeed([FromQuery] PaginationQuery query, 
            CancellationToken cancellationToken)
        {
            var result = await _audioService.GetFeed(_currentUserId, query, cancellationToken);

            return Ok(result);
        }

        [HttpHead("audios/{audioId}/favorite", Name="CheckIfUserFavoritedAudio")]
        public async Task<IActionResult> IsFavorite(string audioId, CancellationToken cancellationToken)
        {
            return await _favoriteService.CheckIfUserFavorited(_currentUserId, audioId, cancellationToken)
                ? NoContent()
                : NotFound();
        }

        [HttpPut("audios/{audioId}/favorite", Name="FavoriteAudio")]
        public async Task<IActionResult> Favorite(string audioId, CancellationToken cancellationToken)
        {
            var result = await _favoriteService.FavoriteAudio(_currentUserId, audioId, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ReturnErrorResponse();
        }

        [HttpDelete("audios/{audioId}/favorite", Name="UnfavoriteAudio")]
        public async Task<IActionResult> Unfavorite(string audioId, CancellationToken cancellationToken)
        {
            var result = await _favoriteService.UnfavoriteAudio(_currentUserId, audioId, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ReturnErrorResponse();
        }
        
        [HttpHead("users/{username}/follow", Name="CheckIfUserFollowedUser")]
        public async Task<IActionResult> IsFollow(string username, CancellationToken cancellationToken)
        {
            return await _followerService.CheckFollowing(_currentUserId, username, cancellationToken)
                ? NoContent()
                : NotFound();
        }

        [HttpPut("users/{username}/follow", Name="FollowUser")]
        public async Task<IActionResult> Follow(string username, CancellationToken cancellationToken)
        {
            var result = await _followerService.Follow(username.ToLower(), cancellationToken);
            return result.IsSuccess ? NoContent() : result.ReturnErrorResponse();
        }

        [HttpDelete("users/{username}/follow", Name="UnfollowUser")]
        public async Task<IActionResult> Unfollow(string username, CancellationToken cancellationToken)
        {
            var result = await _followerService.Unfollow(username.ToLower(), cancellationToken);
            return result.IsSuccess ? NoContent() : result.ReturnErrorResponse();
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserDetailsRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _userService.UpdateUser(_currentUserId, request, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ReturnErrorResponse();
        }

        [HttpPatch("username")]
        public async Task<IActionResult> ChangeUsername([FromBody] UpdateUsernameRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _userService.UpdateUsername(_currentUserId, request.Username!, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ReturnErrorResponse();
        }

        [HttpPatch("email")]
        public async Task<IActionResult> ChangeEmail([FromBody] UpdateEmailRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _userService.UpdateEmail(_currentUserId, request.Email!, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ReturnErrorResponse();
        }

        [HttpPatch("password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _userService.UpdatePassword(_currentUserId, request, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ReturnErrorResponse();
        }
    }
}