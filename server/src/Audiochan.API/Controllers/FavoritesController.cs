using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Extensions;
using Audiochan.Core.Features.Favorites.Audios.CheckIfFavoritedAudio;
using Audiochan.Core.Features.Favorites.Audios.SetFavorite;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class FavoritesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public FavoritesController(ICurrentUserService currentUserService, IMediator mediator)
        {
            _currentUserService = currentUserService;
            _mediator = mediator;
        }

        [HttpHead("audios/{audioId}", Name = "CheckIfUserFavoritedAudio")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Check if the authenticated user favorited an audio.",
            Description = "Requires authentication.",
            OperationId = "CheckIfUserFavoritedAudio",
            Tags = new[] {"favorites"}
        )]
        public async Task<IActionResult> IsFavorite(long audioId, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            return await _mediator.Send(new CheckIfFavoritedAudioCommand(currentUserId, audioId), cancellationToken)
                ? Ok()
                : NotFound();
        }

        [HttpPut("audios/{audioId}", Name = "FavoriteAudio")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [SwaggerOperation(
            Summary = "Favorite an audio",
            Description = "Requires authentication.",
            OperationId = "FavoriteAudio",
            Tags = new[] {"favorites"}
        )]
        public async Task<IActionResult> Favorite(long audioId, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var result = await _mediator.Send(new SetFavoriteCommand(currentUserId, audioId, true), cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ReturnErrorResponse();
        }

        [HttpDelete("audios/{audioId}", Name = "UnfavoriteAudio")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [SwaggerOperation(
            Summary = "Unfavorite an audio",
            Description = "Requires authentication.",
            OperationId = "UnfavoriteAudio",
            Tags = new[] {"favorites"}
        )]
        public async Task<IActionResult> Unfavorite(long audioId, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var result = await _mediator.Send(new SetFavoriteCommand(currentUserId, audioId, false), cancellationToken);
            return result.IsSuccess
                ? NoContent()
                : result.ReturnErrorResponse();
        }
    }
}