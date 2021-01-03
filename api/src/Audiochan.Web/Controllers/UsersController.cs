using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Features.Profiles.Models;
using Audiochan.Core.Features.Users.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Audiochan.Web.Controllers
{
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IAudioService _audioService;
        private readonly IFollowerService _followerService;
        private readonly IProfileService _profileService;
        private readonly IAudioFavoriteService _audioFavoriteService;
        private readonly ICurrentUserService _currentUserService;

        public UsersController(IAudioService audioService, IFollowerService followerService, 
            IProfileService profileService, IAudioFavoriteService audioFavoriteService, 
            ICurrentUserService currentUserService)
        {
            _audioService = audioService;
            _followerService = followerService;
            _profileService = profileService;
            _audioFavoriteService = audioFavoriteService;
            _currentUserService = currentUserService;
        }

        [HttpGet("{username}", Name="GetProfile")]
        [ProducesResponseType(typeof(ProfileViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserProfile(string username, CancellationToken cancellationToken)
        {
            var result = await _profileService.GetProfile(username, cancellationToken);

            return result.IsSuccess
                ? Ok(result.Data)
                : result.ReturnErrorResponse();
        }

        [HttpGet("{username}/audios", Name="GetUserAudios")]
        [ProducesResponseType(typeof(List<AudioListViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserAudios(string username,
            PaginationQuery paginationQuery, CancellationToken cancellationToken)
        {
            var query = new GetAudioListQuery
            {
                Username = username,
                Page = paginationQuery.Page,
                Size = paginationQuery.Size
            };

            var list = await _audioService.GetList(query, cancellationToken);

            return Ok(list);
        }

        [HttpGet("{username}/favorites")]
        [ProducesResponseType(typeof(List<AudioListViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserFavorites(string username, PaginationQuery paginationQuery,
            CancellationToken cancellationToken)
        {
            var result = await _audioFavoriteService.GetUserFavorites(username, paginationQuery, cancellationToken);
            return result.IsSuccess ? Ok(result.Data) : result.ReturnErrorResponse();
        }

        [HttpGet("{username}/followers", Name="GetUserFollowers")]
        [ProducesResponseType(typeof(List<UserViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFollowers(string username, [FromQuery] PaginationQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _followerService.GetUsersFollowers(username.ToLower(), query, cancellationToken);
            return result.IsSuccess ? Ok(result.Data) : result.ReturnErrorResponse();
        }
        
        [HttpGet("{username}/followings", Name="GetUserFollowings")]
        [ProducesResponseType(typeof(List<UserViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFollowings(string username, [FromQuery] PaginationQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _followerService.GetUsersFollowings(username.ToLower(), query, cancellationToken);
            return result.IsSuccess ? Ok(result.Data) : result.ReturnErrorResponse();
        }
        
        [HttpHead("{username}/follow", Name = "CheckIsFollowing")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CheckIsFollowing(string username, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            return await _followerService.CheckFollowing(currentUserId, username, cancellationToken)
                ? NoContent()
                : NotFound();
        }
        
        [HttpPost("{username}/follow", Name="FollowUser")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> FollowUser(string username, CancellationToken cancellationToken)
        {
            var result = await _followerService.Follow(username.ToLower(), cancellationToken);
            return result.IsSuccess ? NoContent() : result.ReturnErrorResponse();
        }
        
        [HttpDelete("{username}/follow", Name="UnfollowUser")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnfollowUser(string username, CancellationToken cancellationToken)
        {
            var result = await _followerService.Unfollow(username.ToLower(), cancellationToken);
            return result.IsSuccess ? NoContent() : result.ReturnErrorResponse();
        }
    }
}