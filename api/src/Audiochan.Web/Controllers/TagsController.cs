using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Tags.Models;
using Audiochan.Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Audiochan.Web.Controllers
{
    [Route("[controller]")]
    public class TagsController : ControllerBase
    {
        private readonly ITagService _tagService;

        public TagsController(ITagService tagService)
        {
            _tagService = tagService;
        }

        [HttpGet(Name="GetPopularTags")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<PopularTagViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPopularTags([FromQuery] PaginationQuery paginationQuery, CancellationToken cancellationToken)
        {
            return Ok((await _tagService.GetPopularTags(paginationQuery, cancellationToken)).Data);
        }
    }
}