using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Extensions;
using Audiochan.Core.Features.FavoritePlaylists.CheckIfPlaylistFavorited;
using Audiochan.Core.Features.FavoritePlaylists.SetFavoritePlaylist;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.API.Controllers.Me
{
    [Area("me")]
    [Authorize]
    [Route("[area]/favorites/playlists")]
    [ProducesResponseType(401)]
    public class FavoritePlaylistsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly long _currentUserId;

        public FavoritePlaylistsController(ICurrentUserService currentUserService, IMediator mediator)
        {
            _mediator = mediator;
            _currentUserId = currentUserService.GetUserId();
        }
        
        [HttpHead("{playlistId:long}", Name = "CheckIfUserFavoritedPlaylist")]
        [SwaggerOperation(
            Summary = "Check if the authenticated user favorited a playlist",
            Description = "Requires authentication.",
            OperationId = "CheckIfUserFavoritedPlaylist",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> IsFavoritePlaylist(long playlistId, CancellationToken cancellationToken)
        {
            var query = new CheckIfPlaylistFavoritedQuery(playlistId, _currentUserId);
            return await _mediator.Send(query, cancellationToken)
                ? Ok()
                : NotFound();
        }

        [HttpPut("{playlistId:long}", Name = "FavoritePlaylist")]
        [SwaggerOperation(
            Summary = "Favorite a playlist",
            Description = "Requires authentication.",
            OperationId = "FavoritePlaylist",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> FavoritePlaylist(long playlistId, CancellationToken cancellationToken)
        {
            var command = new SetFavoritePlaylistCommand(playlistId, _currentUserId, true);
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok() : result.ReturnErrorResponse();
        }
        
        [HttpDelete("{playlistId:long}", Name = "UnfavoritePlaylist")]
        [SwaggerOperation(
            Summary = "Unfavorite a playlist",
            Description = "Requires authentication.",
            OperationId = "UnfavoritePlaylist",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> UnfavoritePlaylist(long playlistId, CancellationToken cancellationToken)
        {
            var command = new SetFavoritePlaylistCommand(playlistId, _currentUserId, false);
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess ? Ok() : result.ReturnErrorResponse();
        }
    }
}