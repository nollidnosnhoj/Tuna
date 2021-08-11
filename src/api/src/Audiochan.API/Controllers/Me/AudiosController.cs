using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Models;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Audios.GetAudioFeed;
using Audiochan.Core.Features.Audios.GetLatestAudios;
using Audiochan.Core.Features.Users.GetUserAudios;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.API.Controllers.Me
{
    [Area("me")]
    [Authorize]
    [Route("[area]/audios")]
    [ProducesResponseType(401)]
    public class MyAudiosController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly long _currentUserId;
        private readonly string _currentUsername;

        public MyAudiosController(ICurrentUserService currentUserService, IMediator mediator)
        {
            _mediator = mediator;
            _currentUsername = currentUserService.GetUsername();
            _currentUserId = currentUserService.GetUserId();
        }
        
        [HttpGet]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [SwaggerOperation(
            Summary = "Get authenticated user's uploaded audios.",
            Description = "Requires authentication",
            OperationId = "YourAudios",
            Tags = new [] {"me"}
        )]
        public async Task<ActionResult<GetAudioListViewModel>> GetYourAudios(
            [FromQuery] OffsetPaginationQueryParams queryParams, 
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetUsersAudioQuery
            {
                Offset = queryParams.Offset,
                Size = queryParams.Size,
                Username = _currentUsername
            }, cancellationToken);
            return Ok(result);
        }
        
        [HttpGet("feed", Name = "GetAuthenticatedUserFeed")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [SwaggerOperation(
            Summary = "Returns a list of tracks uploaded by authenticated user's followings.",
            Description = "Requires authentication.",
            OperationId = "GetYourFeed",
            Tags = new[] {"me"}
        )]
        public async Task<ActionResult<PagedListDto<AudioViewModel>>> GetYourFeed(
            [FromQuery] OffsetPaginationQueryParams queryParams, 
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAudioFeedQuery
            {
                UserId = _currentUserId,
                Offset = queryParams.Offset,
                Size = queryParams.Size
            }, cancellationToken);
            return Ok(result);
        }
    }
}