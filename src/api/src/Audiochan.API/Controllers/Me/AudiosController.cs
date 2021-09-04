using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Models;
using Audiochan.Core.Audios;
using Audiochan.Core.Audios.GetAudioFeed;
using Audiochan.Core.Common.Interfaces.Services;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Core.Users.GetUserAudios;
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
        public async Task<ActionResult<OffsetPagedListDto<AudioDto>>> GetYourAudios(
            [FromQuery] OffsetPaginationQueryParams queryParams, 
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetUsersAudioQuery(_currentUsername)
            {
                Offset = queryParams.Offset,
                Size = queryParams.Size,
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
        public async Task<ActionResult<PagedListDto<AudioDto>>> GetYourFeed(
            [FromQuery] OffsetPaginationQueryParams queryParams, 
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAudioFeedQuery(_currentUserId)
            {
                Offset = queryParams.Offset,
                Size = queryParams.Size
            }, cancellationToken);
            return Ok(result);
        }
    }
}