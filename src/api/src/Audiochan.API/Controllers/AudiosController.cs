using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Extensions;
using Audiochan.API.Models;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Models.Pagination;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.CreateAudio;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Audios.GetAudios;
using Audiochan.Core.Features.Audios.RemoveAudio;
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
        public async Task<ActionResult<CursorPagedListDto<AudioViewModel>>> Get([FromQuery] GetAudiosQuery query, CancellationToken cancellationToken)
        {
            return new JsonResult(await _mediator.Send(query, cancellationToken));
        }

        [AllowAnonymous]
        [HttpGet("{idSlug:idSlug}", Name = "GetAudio")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [SwaggerOperation(Summary = "Return an audio by ID.", OperationId = "GetAudio", Tags = new[] {"audios"})]
        public async Task<ActionResult<AudioViewModel>> Get([FromRoute] string idSlug, [FromQuery] string? secret, CancellationToken cancellationToken)
        {
            var (id, _) = idSlug.ExtractIdAndSlugFromSlug();
            var result = await _mediator.Send(new GetAudioQuery(id, secret), cancellationToken);
            return result != null
                ? Ok(result)
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
        public async Task<ActionResult<AudioViewModel>> Create([FromBody] CreateAudioCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess) return result.ReturnErrorResponse();
            var response = await _mediator.Send(new GetAudioQuery(result.Data, ""), cancellationToken);
            return CreatedAtAction(nameof(Get), new {idSlug = response!.Slug}, response);
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
        public async Task<ActionResult<AudioViewModel>> Update(long audioId,
            [FromBody] UpdateAudioRequest request,
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
        public async Task<ActionResult<ImageUploadResponse>> AddPicture(long audioId,
            [FromBody] ImageUploadRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new UpdateAudioPictureCommand
            {
                AudioId = audioId,
                Data = request.Data
            }, cancellationToken);
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
            var result = await _mediator.Send(new UpdateAudioPictureCommand
            {
                AudioId = audioId,
                Data = string.Empty
            }, cancellationToken);
            
            return result.IsSuccess
                ? NoContent()
                : result.ReturnErrorResponse();
        }
    }
}