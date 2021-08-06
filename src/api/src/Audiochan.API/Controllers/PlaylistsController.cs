using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Extensions;
using Audiochan.API.Models;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Playlists.AddAudiosToPlaylist;
using Audiochan.Core.Features.Playlists.CheckDuplicatedAudios;
using Audiochan.Core.Features.Playlists.CreatePlaylist;
using Audiochan.Core.Features.Playlists.GetPlaylistAudios;
using Audiochan.Core.Features.Playlists.GetPlaylistDetail;
using Audiochan.Core.Features.Playlists.RemoveAudiosFromPlaylist;
using Audiochan.Core.Features.Playlists.RemovePlaylist;
using Audiochan.Core.Features.Playlists.UpdatePlaylistDetails;
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

        [HttpGet("{playlistId:guid}", Name="GetPlaylist")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [SwaggerOperation(Summary = "Return an playlist by ID.", OperationId = "GetPlaylist", Tags = new[] {"playlists"})]
        public async Task<ActionResult<PlaylistViewModel>> GetPlaylist(Guid playlistId, CancellationToken cancellationToken)
        {
            var playlist = await _mediator.Send(new GetPlaylistDetailQuery(playlistId), cancellationToken);
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
        public async Task<ActionResult<PlaylistViewModel>> CreatePlaylist(
            [FromBody] CreatePlaylistCommand command, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess) return result.ReturnErrorResponse();
            var response = await _mediator.Send(new GetPlaylistDetailQuery(result.Data), cancellationToken);
            return CreatedAtAction(nameof(GetPlaylist), new
            {
                playlistId = response!.Id
            }, response);
        }
        
        [Authorize]
        [HttpPut("{playlistId:guid}", Name = "UpdatePlaylistDetails")]
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
        public async Task<IActionResult> UpdatePlaylistDetails(Guid playlistId,
            [FromBody] UpdatePlaylistDetailsRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UpdatePlaylistDetailsCommand(playlistId, request), cancellationToken);

            return result.IsSuccess
                ? Ok(result.Data)
                : result.ReturnErrorResponse();
        }

        [Authorize]
        [HttpDelete("{playlistId:guid}", Name = "RemovePlaylist")]
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
        public async Task<IActionResult> RemovePlaylist(Guid playlistId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RemovePlaylistCommand(playlistId), cancellationToken);

            return result.IsSuccess
                ? NoContent()
                : result.ReturnErrorResponse();
        }

        [HttpGet("{playlistId:guid}/audios", Name = "GetPlaylistAudios")]
        [ProducesResponseType(200)]
        [SwaggerOperation(
            Summary = "Return list of audios from a playlist",
            OperationId = "GetPlaylistAudios",
            Tags = new[] { "playlists" })]
        public async Task<ActionResult<PagedListDto<AudioViewModel>>> GetPlaylistAudios(Guid playlistId,
            CancellationToken cancellationToken)
        {
            var audios = await _mediator.Send(
                new GetPlaylistAudiosQuery(playlistId), cancellationToken);

            return new JsonResult(audios);
        }

        [Authorize]
        [HttpPut("{playlistId:guid}/audios", Name = "AddAudiosToPlaylist")]
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
        public async Task<IActionResult> AddAudiosToPlaylist(Guid playlistId,
            [FromBody] AddAudiosToPlaylistRequest request, CancellationToken cancellationToken)
        {
            var command = new AddAudiosToPlaylistCommand(playlistId, request);
            var result = await _mediator.Send(command, cancellationToken);

            return result.IsSuccess
                ? NoContent()
                : result.ReturnErrorResponse();
        }
        
        [Authorize]
        [HttpDelete("{playlistId:guid}/audios", Name = "RemoveAudiosToPlaylist")]
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
        public async Task<IActionResult> RemoveAudiosToPlaylist(Guid playlistId,
            [FromBody] RemoveAudiosFromPlaylistRequest request, CancellationToken cancellationToken)
        {
            var command = new RemoveAudiosFromPlaylistCommand(playlistId, request);
            var result = await _mediator.Send(command, cancellationToken);

            return result.IsSuccess
                ? NoContent()
                : result.ReturnErrorResponse();
        }
        
        [Authorize]
        [HttpPost("{playlistId:guid}/audios/duplicate", Name = "CheckDuplicatedAudios")]
        [ProducesResponseType(200)]
        [SwaggerOperation(
            Summary = "Check to see if the input audio ids already exist in playlist.",
            Description = "Requires authentication.",
            OperationId = "CheckDuplicatedAudios",
            Tags = new[] {"playlists"}
        )]
        public async Task<IActionResult> CheckDuplicatedAudios(Guid playlistId,
            [FromBody] CheckDuplicatedAudiosRequest request, CancellationToken cancellationToken)
        {
            var command = new CheckDuplicatedAudiosQuery(playlistId, request.AudioIds);
            var result = await _mediator.Send(command, cancellationToken);
            return new JsonResult(result);
        }
    }
}