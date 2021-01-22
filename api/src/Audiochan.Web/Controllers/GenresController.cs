using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Genres.Models;
using Audiochan.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.Web.Controllers
{
    [Route("[controller]")]
    public class GenresController : ControllerBase
    {
        private readonly IGenreService _genreService;

        public GenresController(IGenreService genreService)
        {
            _genreService = genreService;
        }

        [HttpGet(Name = "GetGenres")]
        [ProducesResponseType(typeof(List<Genre>), StatusCodes.Status200OK)]
        [SwaggerOperation(
            Summary = "Returns a list of available genres.",
            OperationId = "GetGenres",
            Tags = new []{"genres"})]
        public async Task<IActionResult> GetGenres([FromQuery] ListGenresQueryParams queryParams, 
            CancellationToken cancellationToken)
        {
            return Ok(await _genreService.ListGenre(queryParams, cancellationToken));
        }
    }
}