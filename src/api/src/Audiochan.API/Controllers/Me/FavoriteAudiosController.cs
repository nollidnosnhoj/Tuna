using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Extensions;
using Audiochan.API.Models;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Users.CheckIfAudioFavorited;
using Audiochan.Core.Users.GetUserFavoriteAudios;
using Audiochan.Core.Users.SetFavoriteAudio;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.API.Controllers.Me
{
    [Area("me")]
    [Authorize]
    [Route("[area]/favorites/audios")]
    [ProducesResponseType(401)]
    public class FavoriteAudiosController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly long _currentUserId;
        private readonly string _currentUsername;

        public FavoriteAudiosController(IAuthService authService, IMediator mediator)
        {
            _mediator = mediator;
            _currentUsername = authService.GetUsername();
            _currentUserId = authService.GetUserId();
        }
        
        [HttpGet(Name = "YourFavoriteAudios")]
        [SwaggerOperation(
            Summary = "Get Your favorite audios",
            Description = "Requires authentication.",
            OperationId = "YourFavoriteAudios",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> GetYourFavoriteAudios([FromQuery] OffsetPaginationQueryParams queryParams,
            CancellationToken cancellationToken = default)
        {
            var query = new GetUserFavoriteAudiosQuery
            {
                Username = _currentUsername,
                Offset = queryParams.Offset,
                Size = queryParams.Size
            };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        
        [HttpHead("{audioId:long}", Name="CheckIfUserFavoritedAudio")]
        [SwaggerOperation(
            Summary = "Check if the authenticated user favorited an audio",
            Description = "Requires authentication.",
            OperationId = "CheckIfUserFavoritedAudio",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> IsFavoriteAudio(long audioId, CancellationToken cancellationToken)
        {
            var command = new CheckIfAudioFavoritedQuery(audioId, _currentUserId);
            return await _mediator.Send(command, cancellationToken)
                ? Ok()
                : NotFound();
        }
        
        [HttpPut("{audioId:long}", Name = "FavoriteAudio")]
        [SwaggerOperation(
            Summary = "Favorite an audio",
            Description = "Requires authentication.",
            OperationId = "FavoriteAudio",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> FavoriteAudio(long audioId, CancellationToken cancellationToken)
        {
            var command = new SetFavoriteAudioCommand(audioId, _currentUserId, true);
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess
                ? Ok()
                : result.ReturnErrorResponse();
        }

        [HttpDelete("{audioId:long}", Name = "UnFavoriteAudio")]
        [SwaggerOperation(
            Summary = "UnFavorite an audio",
            Description = "Requires authentication.",
            OperationId = "UnFavoriteAudio",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> UnFavoriteAudio(long audioId, CancellationToken cancellationToken)
        {
            var command = new SetFavoriteAudioCommand(audioId, _currentUserId, false);
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess
                ? NoContent()
                : result.ReturnErrorResponse();
        }

    }
}