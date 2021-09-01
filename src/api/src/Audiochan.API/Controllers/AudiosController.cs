using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Extensions;
using Audiochan.API.Models;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.CreateAudio;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Audios.GetAudios;
using Audiochan.Core.Features.Audios.RemoveAudio;
using Audiochan.Core.Features.Audios.RemovePicture;
using Audiochan.Core.Features.Audios.UpdateAudio;
using Audiochan.Core.Features.Audios.UpdatePicture;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.API.Controllers
{
    [Authorize]
    [Route("audios")]
    public class AudiosController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AudiosController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [AllowAnonymous]
        [HttpGet(Name = "GetAudios")]
        [ProducesResponseType(200)]
        [SwaggerOperation(Summary = "Return audios by latest.", OperationId = "GetAudios", Tags = new[] {"audios"})]
        public async Task<ActionResult<CursorPagedListDto<AudioDto>>> Get([FromQuery] GetAudiosQuery query, CancellationToken cancellationToken)
        {
            return new JsonResult(await _mediator.Send(query, cancellationToken));
        }

        [AllowAnonymous]
        [HttpGet("{slug}", Name = "GetAudio")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [SwaggerOperation(Summary = "Return an audio by ID.", OperationId = "GetAudio", Tags = new[] {"audios"})]
        public async Task<ActionResult<AudioDto>> Get([FromRoute] string slug, CancellationToken cancellationToken)
        {
            // Decode hash into audio id
            var id = HashIdHelper.DecodeLong(slug);
            
            var result = await _mediator.Send(new GetAudioQuery(id), cancellationToken);
            return result != null
                ? new JsonResult(result)
                : NotFound(ErrorApiResponse.NotFound("Audio was not found."));
        }

        [HttpPost(Name = "CreateAudio")]
        [ProducesResponseType(201)]
        [ProducesResponseType(401)]
        [ProducesResponseType(422)]
        [SwaggerOperation(
            Summary = "Create audio using the provided upload id.",
            Description = "Requires authentication.",
            OperationId = "CreateAudio",
            Tags = new[] {"audios"}
        )]
        public async Task<ActionResult<AudioDto>> Create([FromBody] CreateAudioCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess) return result.ReturnErrorResponse();
            
            // Fetch the newly created audio
            var response = await _mediator.Send(new GetAudioQuery(result.Data), cancellationToken);
            return Created($"/audios/{response!.Slug}", response);
        }

        [HttpPut("{audioId:long}", Name = "UpdateAudio")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [ProducesResponseType(422)]
        [SwaggerOperation(
            Summary = "Update audio's details.",
            Description = "Requires authentication.",
            OperationId = "UpdateAudio",
            Tags = new[] {"audios"})]
        public async Task<ActionResult<AudioDto>> Update(long audioId, [FromBody] UpdateAudioRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(UpdateAudioCommand.FromRequest(audioId, request), cancellationToken);
            return result.IsSuccess ? Ok(result.Data) : result.ReturnErrorResponse();
        }

        [HttpDelete("{audioId:long}", Name = "DeleteAudio")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [SwaggerOperation(
            Summary = "Remove audio.",
            Description = "Requires authentication.",
            OperationId = "DeleteAudio",
            Tags = new[] {"audios"})]
        public async Task<IActionResult> Destroy(long audioId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RemoveAudioCommand(audioId), cancellationToken);
            return result.IsSuccess ? NoContent() : result.ReturnErrorResponse();
        }

        [HttpPatch("{audioId:long}/picture")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [SwaggerOperation(
            Summary = "Add Picture.",
            Description = "Requires authentication.",
            OperationId = "AddAudioPicture",
            Tags = new[] {"audios"})]
        public async Task<ActionResult<ImageUploadResponse>> AddPicture(long audioId, [FromBody] ImageUploadRequest request,
            CancellationToken cancellationToken)
        {
            var command = new UpdateAudioPictureCommand(audioId, request.Data);
            var result = await _mediator.Send(command, cancellationToken);
            return result.IsSuccess
                ? Ok(result.Data)
                : result.ReturnErrorResponse();
        }
        
        [HttpDelete("{audioId:long}/picture")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [SwaggerOperation(
            Summary = "Remove Picture.",
            Description = "Requires authentication.",
            OperationId = "RemoveAudioPicture",
            Tags = new[] {"audios"})]
        public async Task<ActionResult> RemovePicture(long audioId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RemoveAudioPictureCommand(audioId), cancellationToken);
            
            return result.IsSuccess
                ? NoContent()
                : result.ReturnErrorResponse();
        }
    }
}