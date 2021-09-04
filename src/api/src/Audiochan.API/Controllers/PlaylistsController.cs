using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Extensions;
using Audiochan.API.Models;
using Audiochan.Core.Audios;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Core.Playlists;
using Audiochan.Core.Playlists.AddAudiosToPlaylist;
using Audiochan.Core.Playlists.CreatePlaylist;
using Audiochan.Core.Playlists.GetDuplicatedAudiosInPlaylist;
using Audiochan.Core.Playlists.GetPlaylist;
using Audiochan.Core.Playlists.GetPlaylistAudios;
using Audiochan.Core.Playlists.RemoveAudiosFromPlaylist;
using Audiochan.Core.Playlists.RemovePlaylist;
using Audiochan.Core.Playlists.UpdatePlaylistDetails;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.API.Controllers
{
    [Route("playlists")]
    public class PlaylistsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PlaylistsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id:long}", Name="GetPlaylist")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [SwaggerOperation(Summary = "Return an playlist by ID.", OperationId = "GetPlaylist", Tags = new[] {"playlists"})]
        public async Task<ActionResult<PlaylistDto>> GetPlaylist(long id, CancellationToken cancellationToken)
        {

            var playlist = await _mediator.Send(new GetPlaylistQuery(id), cancellationToken);
            return playlist is null
                ? NotFound(ErrorApiResponse.NotFound("Playlist was not found."))
                : Ok(playlist);
        }


        [Authorize]
        [HttpPost(Name = "CreatePlaylist")]
        [ProducesResponseType(201)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [SwaggerOperation(
            Summary = "Create playlist.",
            Description = "Requires authentication.",
            OperationId = "CreatePlaylist",
            Tags = new[] {"playlists"}
        )]
        public async Task<ActionResult<PlaylistDto>> CreatePlaylist(
            [FromBody] CreatePlaylistCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess) return result.ReturnErrorResponse();
            var response = await _mediator.Send(new GetPlaylistQuery(result.Data), cancellationToken);
            return CreatedAtAction(nameof(GetPlaylist), new
            {
                playlistId = response!.Id
            }, response);
        }
        
        [Authorize]
        [HttpPut("{playlistId:long}", Name = "UpdatePlaylistDetails")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(422)]
        [SwaggerOperation(
            Summary = "Update playlist details.",
            Description = "Requires authentication.",
            OperationId = "UpdatePlaylistDetails",
            Tags = new[] {"playlists"}
        )]
        public async Task<IActionResult> UpdatePlaylistDetails(long playlistId,
            [FromBody] UpdatePlaylistDetailsRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UpdatePlaylistDetailsCommand(playlistId, request), cancellationToken);

            return result.IsSuccess
                ? Ok(result.Data)
                : result.ReturnErrorResponse();
        }

        [Authorize]
        [HttpDelete("{playlistId:long}", Name = "RemovePlaylist")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [SwaggerOperation(
            Summary = "Remove playlist.",
            Description = "Requires authentication.",
            OperationId = "RemovePlaylist",
            Tags = new[] {"playlists"}
        )]
        public async Task<IActionResult> RemovePlaylist(long playlistId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RemovePlaylistCommand(playlistId), cancellationToken);

            return result.IsSuccess
                ? NoContent()
                : result.ReturnErrorResponse();
        }

        [HttpGet("{playlistId:long}/audios", Name = "GetPlaylistAudios")]
        [ProducesResponseType(200)]
        [SwaggerOperation(
            Summary = "Return list of audios from a playlist",
            OperationId = "GetPlaylistAudios",
            Tags = new[] { "playlists" })]
        public async Task<ActionResult<PagedListDto<AudioDto>>> GetPlaylistAudios(long playlistId,
            [FromQuery] CursorPaginationQueryParams<long> queryParams,
            CancellationToken cancellationToken)
        {
            var (cursor, size) = queryParams;
            var audios = await _mediator.Send(new GetPlaylistAudiosQuery(playlistId)
            {
                Cursor = cursor,
                Size = size
            }, cancellationToken);

            return new JsonResult(audios);
        }

        [Authorize]
        [HttpPut("{playlistId:long}/audios", Name = "AddAudiosToPlaylist")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [SwaggerOperation(
            Summary = "Add audios to playlist.",
            Description = "Requires authentication.",
            OperationId = "AddAudiosToPlaylist",
            Tags = new[] {"playlists"}
        )]
        public async Task<IActionResult> AddAudiosToPlaylist(long playlistId,
            [FromBody] AddAudiosToPlaylistRequest request, CancellationToken cancellationToken)
        {
            var command = new AddAudiosToPlaylistCommand(playlistId, request.AudioIds);
            var result = await _mediator.Send(command, cancellationToken);

            return result.IsSuccess
                ? NoContent()
                : result.ReturnErrorResponse();
        }
        
        [Authorize]
        [HttpDelete("{playlistId:long}/audios", Name = "RemoveAudiosToPlaylist")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [SwaggerOperation(
            Summary = "Remove audios to playlist.",
            Description = "Requires authentication.",
            OperationId = "RemoveAudiosToPlaylist",
            Tags = new[] {"playlists"}
        )]
        public async Task<IActionResult> RemoveAudiosToPlaylist(long playlistId,
            [FromBody] RemoveAudiosFromPlaylistRequest request, CancellationToken cancellationToken)
        {
            var command = new RemoveAudiosFromPlaylistCommand(playlistId, request.PlaylistAudioIds);
            var result = await _mediator.Send(command, cancellationToken);

            return result.IsSuccess
                ? NoContent()
                : result.ReturnErrorResponse();
        }
        
        [Authorize]
        [HttpPost("{playlistId:long}/audios/duplicate", Name = "CheckDuplicatedAudios")]
        [ProducesResponseType(200)]
        [SwaggerOperation(
            Summary = "Check to see if the input audio ids already exist in playlist.",
            Description = "Requires authentication.",
            OperationId = "CheckDuplicatedAudios",
            Tags = new[] {"playlists"}
        )]
        public async Task<IActionResult> CheckDuplicatedAudios(long playlistId,
            [FromBody] CheckDuplicatedAudiosRequest request, CancellationToken cancellationToken)
        {
            var command = new GetDuplicatedAudiosInPlaylistQuery(playlistId, request.AudioIds);
            var result = await _mediator.Send(command, cancellationToken);
            return new JsonResult(result);
        }
    }
}