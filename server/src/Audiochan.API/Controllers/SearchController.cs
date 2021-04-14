using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Models.ViewModels;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Search.SearchAudios;
using Audiochan.Core.Features.Search.SearchUsers;
using Audiochan.Core.Features.Users.GetUser;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.API.Controllers
{
    [Route("search")]
    public class SearchController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SearchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("audios")]
        [ProducesResponseType(typeof(PagedList<AudioDetailViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Search for audios", OperationId = "SearchAudio", Tags = new []{"search"})]
        public async Task<IActionResult> SearchAudios([FromQuery] SearchAudiosRequest request,
            CancellationToken cancellationToken)
        {
            var results = await _mediator.Send(request, cancellationToken);
            return new JsonResult(results);
        }

        [HttpGet("users")]
        [ProducesResponseType(typeof(PagedList<UserViewModel>), StatusCodes.Status200OK)]
        [SwaggerOperation(Summary = "Search for users", OperationId = "SearchUsers", Tags = new []{"search"})]
        public async Task<IActionResult> SearchUsers([FromQuery] SearchUsersRequest request,
            CancellationToken cancellationToken)
        {
            var results = await _mediator.Send(request, cancellationToken);
            return new JsonResult(results);
        }
    }
}