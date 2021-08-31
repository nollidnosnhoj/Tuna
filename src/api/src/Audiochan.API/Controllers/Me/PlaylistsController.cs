using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Models;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Core.Features.Playlists;
using Audiochan.Core.Features.Playlists.GetPlaylist;
using Audiochan.Core.Features.Users.GetUserPlaylists;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.API.Controllers.Me
{
    [Area("me")]
    [Authorize]
    [Route("[area]/playlists")]
    public class PlaylistsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly long _currentUserId;
        private readonly string _currentUsername;

        public PlaylistsController(ICurrentUserService currentUserService, IMediator mediator)
        {
            _mediator = mediator;
            _currentUsername = currentUserService.GetUsername();
            _currentUserId = currentUserService.GetUserId();
        }

        [HttpGet(Name="GetYourPlaylists")]
        [ProducesResponseType(200)]
        [SwaggerOperation(Summary = "Get user's playlists", OperationId = "GetYourPlaylists", Tags = new []{"me"})]
        public async Task<ActionResult<PagedListDto<PlaylistDto>>> GetYourPlaylists(
            [FromQuery] OffsetPaginationQueryParams paginationQueryParams,
            CancellationToken cancellationToken)
        {
            var audios = await _mediator.Send(
                new GetUserPlaylistsQuery(_currentUsername, paginationQueryParams),
                cancellationToken);

            return new JsonResult(audios);
        }
    }
}