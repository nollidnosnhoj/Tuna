using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Features.Audios.UploadAudio;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.API.Controllers
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
        public async Task<IActionResult> GetUploadUrl([FromBody] UploadAudioRequest request, CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(request, cancellationToken);
            return new JsonResult(response);
        }
    }
}