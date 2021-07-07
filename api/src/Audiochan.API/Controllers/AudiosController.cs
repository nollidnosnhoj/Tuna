using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Extensions;
using Audiochan.API.Models;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.CreateAudio;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Features.Audios.RemoveAudio;
using Audiochan.Core.Features.Audios.UpdateAudio;
using Audiochan.Core.Features.Audios.UpdatePicture;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
        [SwaggerOperation(Summary = "Returns a list of audios.", OperationId = "GetAudios", Tags = new[] {"audios"})]
        public async Task<ActionResult<PagedListDto<AudioViewModel>>> GetList([FromQuery] GetLatestAudioQuery query,
            CancellationToken cancellationToken)
        {
            return Ok(await _mediator.Send(query, cancellationToken));
        }

        [AllowAnonymous]
        [HttpGet("{audioId:guid}", Name = "GetAudio")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [SwaggerOperation(Summary = "Return an audio by ID.", OperationId = "GetAudio", Tags = new[] {"audios"})]
        public async Task<ActionResult<AudioDetailViewModel>> Get(Guid audioId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAudioQuery(audioId), cancellationToken);
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
        public async Task<ActionResult<AudioDetailViewModel>> Create([FromBody] CreateAudioCommand command,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(command, cancellationToken);
            if (!result.IsSuccess) return result.ReturnErrorResponse();
            var response = await _mediator.Send(new GetAudioQuery(result.Data), cancellationToken);
            return CreatedAtAction(nameof(Get), new {audioId = response!.Id}, response);
        }

        [HttpPut("{audioId:guid}", Name = "UpdateAudio")]
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
        public async Task<ActionResult<AudioDetailViewModel>> Update(Guid audioId,
            [FromBody] UpdateAudioRequest request,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(UpdateAudioCommand.FromRequest(audioId, request), cancellationToken);
            return result.IsSuccess ? Ok(result.Data) : result.ReturnErrorResponse();
        }

        [HttpDelete("{audioId:guid}", Name = "DeleteAudio")]
        [ProducesResponseType(204)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [SwaggerOperation(
            Summary = "Remove audio.",
            Description = "Requires authentication.",
            OperationId = "DeleteAudio",
            Tags = new[] {"audios"})]
        public async Task<IActionResult> Destroy(Guid audioId, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RemoveAudioCommand(audioId), cancellationToken);
            return result.IsSuccess ? NoContent() : result.ReturnErrorResponse();
        }

        [HttpPatch("{audioId:guid}/picture")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        [SwaggerOperation(
            Summary = "Add Picture.",
            Description = "Requires authentication.",
            OperationId = "AddAudioPicture",
            Tags = new[] {"audios"})]
        public async Task<ActionResult<ImageUploadResponse>> AddPicture(Guid audioId,
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
    }
}