using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Extensions;
using Audiochan.API.Models;
using Audiochan.Core.Features.Users.CheckIfAudioFavorited;
using Audiochan.Core.Features.Users.GetUserFavoriteAudios;
using Audiochan.Core.Features.Users.SetFavoriteAudio;
using Audiochan.Core.Interfaces;
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

        public FavoriteAudiosController(ICurrentUserService currentUserService, IMediator mediator)
        {
            _mediator = mediator;
            _currentUsername = currentUserService.GetUsername();
            _currentUserId = currentUserService.GetUserId();
        }
        
        [HttpGet]
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

        [HttpDelete("{audioId:long}", Name = "UnfavoriteAudio")]
        [SwaggerOperation(
            Summary = "Unfavorite an audio",
            Description = "Requires authentication.",
            OperationId = "UnfavoriteAudio",
            Tags = new[] {"me"}
        )]
        public async Task<IActionResult> UnfavoriteAudio(long audioId, CancellationToken cancellationToken)
        {
            var command = new SetFavoriteAudioCommand(audioId, _currentUserId, false);
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess
                ? NoContent()
                : result.ReturnErrorResponse();
        }

    }
}