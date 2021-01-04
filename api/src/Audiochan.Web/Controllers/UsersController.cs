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
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IAudioService _audioService;
        private readonly IFollowerService _followerService;
        private readonly IFavoriteService _favoriteService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserService _userService;

        public UsersController(IAudioService audioService, IFollowerService followerService, 
            IFavoriteService favoriteService, ICurrentUserService currentUserService, 
            IUserService userService)
        {
            _audioService = audioService;
            _followerService = followerService;
            _favoriteService = favoriteService;
            _currentUserService = currentUserService;
            _userService = userService;
        }

        [HttpGet("{username}", Name="GetProfile")]
        [ProducesResponseType(typeof(UserDetailsViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserProfile(string username, CancellationToken cancellationToken)
        {
            var result = await _userService.GetUserDetails(username, cancellationToken);

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
            var result = await _favoriteService.GetUserFavorites(username, paginationQuery, cancellationToken);
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
    }
}