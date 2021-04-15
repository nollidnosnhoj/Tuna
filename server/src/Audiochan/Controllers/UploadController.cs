using System.Threading;
using System.Threading.Tasks;
using Audiochan.Features.Audios.GetUploadAudioUrl;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class UploadController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UploadController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Get a temporary pre-signed AWS S3 Put link to upload audio.",
            OperationId = "GetPresignedUrl",
            Tags = new[] {"upload"}
        )]
        public async Task<IActionResult> GetUploadUrl([FromBody] GetUploadAudioUrlRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return new JsonResult(response);
        }
    }
}