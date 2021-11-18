using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Models;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Core.Users.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

// ReSharper disable RouteTemplates.ActionRoutePrefixCanBeExtractedToControllerRoute

namespace Audiochan.API.Controllers
{
    [Route("users")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{username}/favorite/audios", Name = "GetUserFavoriteAudios")]
        [ProducesResponseType(200)]
        [SwaggerOperation(
            Summary = "Returns a list of user's favorite audios.",
            OperationId = "GetUsersFavoriteAudios",
            Tags=new []{"users"})]
        public async Task<IActionResult> GetUserFavoriteAudios(string username,
            [FromQuery] OffsetPaginationQueryParams paginationQueryParams, CancellationToken cancellationToken)
        {
            var list = await _mediator.Send(new GetUserFavoriteAudiosQuery
            {
                Username = username,
                Offset = paginationQueryParams.Offset,
                Size = paginationQueryParams.Size
            }, cancellationToken);
            return Ok(list);
        }

        [HttpGet("{username}/followings", Name = "GetUserFollowings")]
        [ProducesResponseType(200)]
        [SwaggerOperation(Summary = "Return a list of the user's followings.", OperationId = "GetUserFollowings",
            Tags = new[] {"users"})]
        public async Task<ActionResult<PagedListDto<FollowingViewModel>>> GetFollowings(string username, 
            [FromQuery] OffsetPaginationQueryParams paginationQueryParams, 
            CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetUserFollowingsQuery
            {
                Username = username,
                Offset = paginationQueryParams.Offset,
                Size = paginationQueryParams.Size
            }, cancellationToken));
        }
    }
}