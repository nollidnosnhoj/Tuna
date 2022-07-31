using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Extensions;
using Audiochan.API.Features.Upload.Commands.CreateUpload;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.API.Controllers
{
    [Authorize]
    [Route("upload")]
    public class UploadController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UploadController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [SwaggerOperation(
            Summary = "Request a upload link for the audio.",
            Description = "Requires authentication.",
            OperationId = "RequestUploadUrl",
            Tags = new []{"upload"}
        )]
        public async Task<ActionResult<GenerateUploadLinkResponse>> GetUploadUrl(
            [FromBody] GenerateUploadLinkCommand command, 
            CancellationToken cancellationToken)
        {
            var response = await _mediator.Send(command, cancellationToken);
            return response.IsSuccess 
                ? new JsonResult(response.Data) 
                : response.ReturnErrorResponse();
        }
    }
}