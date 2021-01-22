using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.Web.Controllers
{
    [Route("[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly IAudioService _audioService;

        public TagsController(IAudioService audioService)
        {
            _audioService = audioService;
        }

        [HttpGet(Name="GetPopularTags")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(PagedList<PopularTagViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(
            Summary = "Return a list of tags, sorted by popularity.",
            OperationId = "GetPopularTags",
            Tags = new []{"tags"}
        )]
        public async Task<IActionResult> GetPopularTags([FromQuery] PaginationQuery paginationQuery, CancellationToken cancellationToken)
        {
            return Ok(await _audioService.GetPopularTags(paginationQuery, cancellationToken));
        }
    }
}