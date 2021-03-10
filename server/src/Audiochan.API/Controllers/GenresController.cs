using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Genres.ListGenre;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.API.Controllers
{
    [Route("[controller]")]
    public class GenresController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GenresController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet(Name = "GetGenres")]
        [ProducesResponseType(typeof(List<Genre>), StatusCodes.Status200OK)]
        [SwaggerOperation(
            Summary = "Returns a list of available genres.",
            OperationId = "GetGenres",
            Tags = new[] {"genres"})]
        public async Task<IActionResult> GetGenres([FromQuery] ListGenreQuery queryParams,
            CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(queryParams, cancellationToken));
        }
    }
}