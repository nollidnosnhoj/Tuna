using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Extensions;
using Audiochan.API.Models;
using Audiochan.Application.Commons.Dtos.Requests;
using Audiochan.Application.Commons.Dtos.Responses;
using Audiochan.Application.Commons.Dtos.Wrappers;
using Audiochan.Application.Commons.Extensions;
using Audiochan.Application.Commons.Helpers;
using Audiochan.Application.Commons.Interfaces;
using Audiochan.Application.Features.Audios.Commands.CreateAudio;
using Audiochan.Application.Features.Audios.Commands.RemoveAudio;
using Audiochan.Application.Features.Audios.Commands.RemovePicture;
using Audiochan.Application.Features.Audios.Commands.UpdateAudio;
using Audiochan.Application.Features.Audios.Commands.UpdatePicture;
using Audiochan.Application.Features.Audios.Models;
using Audiochan.Application.Features.Audios.Queries.GetAudio;
using Audiochan.Application.Features.Audios.Queries.GetAudios;
using AutoMapper;
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
        private readonly IMapper _mapper;

        public AudiosController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }
        
        [AllowAnonymous]
        [HttpGet(Name = "GetAudios")]
        [ProducesResponseType(200)]
        [SwaggerOperation(Summary = "Return audios by latest.", OperationId = "GetAudios", Tags = new[] {"audios"})]
        public async Task<ActionResult<CursorPagedListDto<AudioDto, long>>> Get([FromQuery] GetAudiosQuery query, CancellationToken cancellationToken)
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
                : NotFound(new ErrorApiResponse("Audio was not found.", null));
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
        public async Task<ActionResult<AudioDto>> Create([FromBody] CreateAudioInput input,
            CancellationToken cancellationToken)
        {
            var userId = User.GetUserId();
            var result = await _mediator.Send(input.ToCommand(userId), cancellationToken);
            if (!result.Succeeded)
            {
                return ((IErrorResult) result).ToErrorObjectResult();
            }

            var response = _mapper.Map<AudioDto>(result.Data);
            
            return Created($"/audios/{response.Slug}", response);
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

            return result.ToObjectResult(Ok);
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
            return result.ToObjectResult(NoContent);
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
            return result.ToObjectResult(Ok);
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
        public async Task<IActionResult> RemovePicture(long audioId,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new RemoveAudioPictureCommand(audioId), cancellationToken);
            return result.ToObjectResult(NoContent);
        }
    }
}