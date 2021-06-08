using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Models;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Features.Followers.GetFollowers;
using Audiochan.Core.Features.Followers.GetFollowings;
using Audiochan.Core.Features.Shared.Responses;
using Audiochan.Core.Features.Users.GetProfile;
using Audiochan.Core.Features.Users.GetUserAudios;
using Audiochan.Core.Features.Users.GetUserFavoriteAudios;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.API.Controllers
{
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{username}", Name = "GetProfile")]
        [ProducesResponseType(typeof(ProfileViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Return user's profile.", OperationId = "GetProfile", Tags = new[] {"users"})]
        public async Task<IActionResult> GetUser(string username, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetProfileQuery(username), cancellationToken);

            return result != null
                ? Ok(result)
                : NotFound(ErrorApiResponse.NotFound("User was not found."));
        }

        [HttpGet("{username}/audios", Name = "GetUserAudios")]
        [ProducesResponseType(typeof(PagedListDto<AudioViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorApiResponse), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Return a list of the user's audios.", OperationId = "GetUserAudios",
            Tags = new[] {"users"})]
        public async Task<IActionResult> GetUserAudios(string username, [FromQuery] PaginationQueryParams paginationQueryParams,
            CancellationToken cancellationToken)
        {
            var list = await _mediator.Send(new GetUsersAudioQuery
            {
                Username = username,
                Page = paginationQueryParams.Page,
                Size = paginationQueryParams.Size
            }, cancellationToken);

            return Ok(list);
        }

        [HttpGet("{username}/favorite/audios", Name = "GetUserFavoriteAudios")]
        public async Task<IActionResult> GetUserFavoriteAudios(string username,
            [FromQuery] PaginationQueryParams paginationQueryParams, CancellationToken cancellationToken)
        {
            var list = await _mediator.Send(new GetUserFavoriteAudiosQuery
            {
                Username = username,
                Page = paginationQueryParams.Page,
                Size = paginationQueryParams.Size
            }, cancellationToken);
            return Ok(list);
        }

        [HttpGet("{username}/followers", Name = "GetUserFollowers")]
        [ProducesResponseType(typeof(PagedListDto<MetaAuthorDto>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Return a list of the user's followers.", OperationId = "GetUserFollowers",
            Tags = new[] {"users"})]
        public async Task<IActionResult> GetFollowers(string username, [FromQuery] PaginationQueryParams paginationQueryParams, 
            CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetUserFollowersQuery
            {
                Username = username,
                Page = paginationQueryParams.Page,
                Size = paginationQueryParams.Size
            }, cancellationToken));
        }

        [HttpGet("{username}/followings", Name = "GetUserFollowings")]
        [ProducesResponseType(typeof(PagedListDto<MetaAuthorDto>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Return a list of the user's followings.", OperationId = "GetUserFollowings",
            Tags = new[] {"users"})]
        public async Task<IActionResult> GetFollowings(string username, [FromQuery] PaginationQueryParams paginationQueryParams, 
            CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetUserFollowingsQuery
            {
                Username = username,
                Page = paginationQueryParams.Page,
                Size = paginationQueryParams.Size
            }, cancellationToken));
        }
    }
}