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
        private readonly ICurrentUserService _currentUserService;
        private readonly IAudioService _audioService;
        private readonly IUserService _userService;

        public MeController(ICurrentUserService currentUserService, IAudioService audioService, 
            IUserService userService)
        {
            _currentUserService = currentUserService;
            _audioService = audioService;
            _userService = userService;
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
            var result = await _userService.GetCurrentUser(cancellationToken);
            
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
            var userId = _currentUserService.GetUserId();
            var result = await _audioService.GetFeed(userId, query, cancellationToken);
            
            return result.IsSuccess 
                ? Ok(result.Data) 
                : result.ReturnErrorResponse();
        }
    }
}