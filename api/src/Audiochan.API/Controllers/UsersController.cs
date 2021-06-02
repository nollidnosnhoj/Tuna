using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Features.Audios.GetAudioList;
using Audiochan.API.Features.Followers.GetFollowers;
using Audiochan.API.Features.Followers.GetFollowings;
using Audiochan.API.Features.Shared.Responses;
using Audiochan.API.Features.Users.GetProfile;
using Audiochan.API.Features.Users.GetUserAudios;
using Audiochan.API.Features.Users.GetUserFavoriteAudios;
using Audiochan.API.Models;
using Audiochan.Core.Models;
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
            var result = await _mediator.Send(new GetProfileRequest(username), cancellationToken);

            return result != null
                ? Ok(result)
                : NotFound(ErrorApiResponse.NotFound("User was not found."));
        }

        [HttpGet("{username}/audios", Name = "GetUserAudios")]
        [ProducesResponseType(typeof(PagedListDto<AudioViewModel>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorApiResponse), StatusCodes.Status404NotFound)]
        [SwaggerOperation(Summary = "Return a list of the user's audios.", OperationId = "GetUserAudios",
            Tags = new[] {"users"})]
        public async Task<IActionResult> GetUserAudios(string username, [FromQuery] GetUserAudiosRequest request,
            CancellationToken cancellationToken)
        {
            request.Username = username;
            var list = await _mediator.Send(request, cancellationToken);

            return Ok(list);
        }

        [HttpGet("{username}/favorite/audios", Name = "GetUserFavoriteAudios")]
        public async Task<IActionResult> GetUserFavoriteAudios(string username,
            [FromQuery] GetUserFavoriteAudiosRequest request, CancellationToken cancellationToken)
        {
            request.Username = username;
            var list = await _mediator.Send(request, cancellationToken);

            return Ok(list);
        }

        [HttpGet("{username}/followers", Name = "GetUserFollowers")]
        [ProducesResponseType(typeof(PagedListDto<MetaAuthorDto>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Return a list of the user's followers.", OperationId = "GetUserFollowers",
            Tags = new[] {"users"})]
        public async Task<IActionResult> GetFollowers(string username, [FromQuery] int page, [FromQuery] int size, 
            CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetUserFollowersRequest
            {
                Username = username,
                Page = page,
                Size = size
            }, cancellationToken));
        }

        [HttpGet("{username}/followings", Name = "GetUserFollowings")]
        [ProducesResponseType(typeof(PagedListDto<MetaAuthorDto>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Return a list of the user's followings.", OperationId = "GetUserFollowings",
            Tags = new[] {"users"})]
        public async Task<IActionResult> GetFollowings(string username, [FromQuery] int page, [FromQuery] int size, 
            CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetUserFollowingsRequest
            {
                Username = username,
                Page = page,
                Size = size
            }, cancellationToken));
        }
    }
}