using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Features.Users.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Web.Extensions;
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
        private readonly IUserService _userService;

        public UsersController(IFollowerService followerService, 
            IFavoriteService favoriteService, IUserService userService, 
            IAudioService audioService)
        {
            _followerService = followerService;
            _favoriteService = favoriteService;
            _userService = userService;
            _audioService = audioService;
        }

        [HttpGet("{username}", Name="GetProfile")]
        [ProducesResponseType(typeof(UserDetailsViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
                Limit = paginationQuery.Limit
            };

            var list = await _audioService.GetList(query, cancellationToken);

            return Ok(list);
        }

        [HttpGet("{username}/favorites")]
        [ProducesResponseType(typeof(List<AudioListViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserFavorites(string username, PaginationQuery paginationQuery,
            CancellationToken cancellationToken)
        {
            return Ok(await _favoriteService.GetUserFavorites(username, paginationQuery, cancellationToken));
        }

        [HttpGet("{username}/followers", Name="GetUserFollowers")]
        [ProducesResponseType(typeof(List<UserViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFollowers(string username, [FromQuery] PaginationQuery query,
            CancellationToken cancellationToken)
        {
            return Ok(await _followerService.GetUsersFollowers(username.ToLower(), query, cancellationToken));
        }
        
        [HttpGet("{username}/followings", Name="GetUserFollowings")]
        [ProducesResponseType(typeof(List<UserViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFollowings(string username, [FromQuery] PaginationQuery query,
            CancellationToken cancellationToken)
        {
            return Ok(await _followerService.GetUsersFollowings(username.ToLower(), query, cancellationToken));
        }
    }
}