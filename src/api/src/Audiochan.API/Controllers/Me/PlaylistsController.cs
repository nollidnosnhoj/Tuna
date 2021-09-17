using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Models;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Core.Playlists;
using Audiochan.Core.Users.GetUserPlaylists;
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

        public PlaylistsController(IAuthService authService, IMediator mediator)
        {
            _mediator = mediator;
            _currentUsername = authService.GetUsername();
            _currentUserId = authService.GetUserId();
        }

        [HttpGet(Name="GetYourPlaylists")]
        [ProducesResponseType(200)]
        [SwaggerOperation(Summary = "Get user's playlists", OperationId = "GetYourPlaylists", Tags = new []{"me"})]
        public async Task<ActionResult<PagedListDto<PlaylistDto>>> GetYourPlaylists(
            [FromQuery] OffsetPaginationQueryParams paginationQueryParams,
            CancellationToken cancellationToken)
        {
            var audios = await _mediator.Send(
                new GetUserPlaylistsQuery(_currentUsername)
                {
                    Offset = paginationQueryParams.Offset,
                    Size = paginationQueryParams.Size
                },
                cancellationToken);

            return new JsonResult(audios);
        }
    }
}