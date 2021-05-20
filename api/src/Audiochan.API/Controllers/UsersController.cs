using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Models;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Followers.GetFollowers;
using Audiochan.Core.Features.Followers.GetFollowings;
using Audiochan.Core.Features.Users;
using Audiochan.Core.Features.Users.GetUser;
using Audiochan.Core.Features.Users.GetUserAudios;
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
        [ProducesResponseType(typeof(UserViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Return user's profile.", OperationId = "GetProfile", Tags = new[] {"users"})]
        public async Task<IActionResult> GetUser(string username, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetUserRequest(username), cancellationToken);

            return result != null
                ? Ok(result)
                : NotFound(ErrorViewModel.NotFound("User was not found."));
        }

        [HttpGet("{username}/audios", Name = "GetUserAudios")]
        [ProducesResponseType(typeof(PagedList<AudioViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Return a list of the user's audios.", OperationId = "GetUserAudios",
            Tags = new[] {"users"})]
        public async Task<IActionResult> GetUserAudios(string username,
            [FromQuery] GetUserAudiosRequest request,
            CancellationToken cancellationToken)
        {
            request.Username = username;
            var list = await _mediator.Send(request, cancellationToken);

            return Ok(list);
        }

        [HttpGet("{username}/followers", Name = "GetUserFollowers")]
        [ProducesResponseType(typeof(PagedList<MetaAuthorDto>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Return a list of the user's followers.", OperationId = "GetUserFollowers",
            Tags = new[] {"users"})]
        public async Task<IActionResult> GetFollowers(string username,
            [FromQuery] PaginationQueryParams query,
            CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetUserFollowersRequest
            {
                Username = username,
                Page = query.Page,
                Size = query.Size
            }, cancellationToken));
        }

        [HttpGet("{username}/followings", Name = "GetUserFollowings")]
        [ProducesResponseType(typeof(PagedList<MetaAuthorDto>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Return a list of the user's followings.", OperationId = "GetUserFollowings",
            Tags = new[] {"users"})]
        public async Task<IActionResult> GetFollowings(string username,
            [FromQuery] PaginationQueryParams query,
            CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetUserFollowingsRequest
            {
                Username = username,
                Page = query.Page,
                Size = query.Size
            }, cancellationToken));
        }
    }
}