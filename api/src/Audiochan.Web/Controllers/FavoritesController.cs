using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Interfaces;
using Audiochan.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public class FavoritesController : ControllerBase
    {
        private readonly IFavoriteService _favoriteService;
        private readonly ICurrentUserService _currentUserService;

        public FavoritesController(IFavoriteService favoriteService, ICurrentUserService currentUserService)
        {
            _favoriteService = favoriteService;
            _currentUserService = currentUserService;
        }

        [HttpHead("audios/{audioId}", Name="CheckIfUserFavoritedAudio")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Check if the authenticated user favorited an audio.",
            Description = "Requires authentication.",
            OperationId = "CheckIfUserFavoritedAudio",
            Tags = new []{"favorites"}
        )]
        public async Task<IActionResult> IsFavorite(string audioId, CancellationToken cancellationToken)
        {
            return await _favoriteService.CheckIfUserFavorited(_currentUserService.GetUserId(), audioId, cancellationToken)
                ? Ok()
                : NotFound();
        }

        [HttpPut("audios/{audioId}", Name="FavoriteAudio")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [SwaggerOperation(
            Summary = "Favorite an audio",
            Description = "Requires authentication.",
            OperationId = "FavoriteAudio",
            Tags = new []{"favorites"}
        )]
        public async Task<IActionResult> Favorite(string audioId, CancellationToken cancellationToken)
        {
            var result = await _favoriteService.FavoriteAudio(_currentUserService.GetUserId(), audioId, cancellationToken);
            return result.IsSuccess 
                ? Ok() 
                : result.ReturnErrorResponse();
        }

        [HttpDelete("audios/{audioId}", Name="UnfavoriteAudio")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [SwaggerOperation(
            Summary = "Unfavorite an audio",
            Description = "Requires authentication.",
            OperationId = "UnfavoriteAudio",
            Tags = new []{"favorites"}
        )]
        public async Task<IActionResult> Unfavorite(string audioId, CancellationToken cancellationToken)
        {
            var result = await _favoriteService.UnfavoriteAudio(_currentUserService.GetUserId(), audioId, cancellationToken);
            return result.IsSuccess 
                ? NoContent() 
                : result.ReturnErrorResponse();
        }
    }
}