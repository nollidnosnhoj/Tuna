using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Models;
using Audiochan.Core.Artists.Queries;
using Audiochan.Core.Audios;
using Audiochan.Core.Common.Models.Pagination;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

// ReSharper disable RouteTemplates.ActionRoutePrefixCanBeExtractedToControllerRoute

namespace Audiochan.API.Controllers
{
    [Route("artists")]
    public class ArtistsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ArtistsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{username}", Name = "GetArtistProfile")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [SwaggerOperation(Summary = "Return artist's profile.", OperationId = "GetArtistProfile", Tags = new[] {"artists"})]
        public async Task<ActionResult<ArtistProfileDto>> GetArtist(string username, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetArtistProfileQuery(username), cancellationToken);

            return result != null
                ? Ok(result)
                : NotFound(ErrorApiResponse.NotFound("User was not found."));
        }

        [HttpGet("{username}/audios", Name = "GetArtistAudios")]
        [ProducesResponseType(200)]
        [SwaggerOperation(Summary = "Return a list of the artist's audios.", OperationId = "GetUserAudios",
            Tags = new[] {"artists"})]
        public async Task<ActionResult<PagedListDto<AudioDto>>> GetArtistAudios(string username, 
            [FromQuery] OffsetPaginationQueryParams paginationQueryParams,
            CancellationToken cancellationToken)
        {
            var list = await _mediator.Send(new GetUsersAudioQuery(username)
            {
                Offset = paginationQueryParams.Offset,
                Size = paginationQueryParams.Size
            }, cancellationToken);

            return Ok(list);
        }

        [HttpGet("{username}/followers", Name = "GetArtistFollowers")]
        [ProducesResponseType(200)]
        [SwaggerOperation(Summary = "Return a list of the artists's followers.", OperationId = "GetArtistFollowers",
            Tags = new[] {"artists"})]
        public async Task<ActionResult<PagedListDto<FollowerViewModel>>> GetFollowers(string username, 
            [FromQuery] OffsetPaginationQueryParams paginationQueryParams, 
            CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(new GetArtistFollowersQuery(username)
            {
                Offset = paginationQueryParams.Offset,
                Size = paginationQueryParams.Size
            }, cancellationToken));
        }
    }
}