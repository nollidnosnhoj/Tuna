using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Interfaces;
using Audiochan.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Audiochan.Web.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class AudiosController : ControllerBase
    {
        private readonly IAudioService _audioService;

        public AudiosController(IAudioService audioService)
        {
            _audioService = audioService;

        }

        [HttpGet(Name="GetAudios")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<AudioListViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetList([FromQuery] GetAudioListQuery query, 
            CancellationToken cancellationToken)
        {
            return Ok(await _audioService.GetList(query, cancellationToken));
        }

        [HttpGet("{audioId}", Name = "GetAudioById")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AudioDetailViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string audioId, CancellationToken cancellationToken)
        {
            var result = await _audioService.Get(audioId, cancellationToken);
            return result.IsSuccess ? Ok(result.Data) : result.ReturnErrorResponse();
        }

        [HttpPost("{audioId}/views", Name = "AddView")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status304NotModified)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddView(string audioId, CancellationToken cancellationToken)
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
            if (string.IsNullOrEmpty(ip)) return BadRequest();
            var result = await _audioService.AddView(audioId, ip, cancellationToken);

            if (result.IsSuccess)
                return result.Data 
                    ? Ok() 
                    : StatusCode(StatusCodes.Status304NotModified);

            return result.ReturnErrorResponse();
        }

        [HttpGet("random-id")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRandomId(CancellationToken cancellationToken)
        {
            var id = await _audioService.GetRandomAudioId(cancellationToken);
            return id == null ? NotFound() : Ok(id);
        }

        [HttpPost(Name="UploadAudio")]
        [ProducesResponseType(typeof(AudioDetailViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Upload(
            [FromForm] UploadAudioRequest request
            , CancellationToken cancellationToken)
        {
            var result = await _audioService.Create(request, cancellationToken);
            return result.IsSuccess 
                ? CreatedAtRoute(new {result.Data.Id}, result.Data) 
                : result.ReturnErrorResponse();
        }

        [HttpPatch("{audioId}", Name="UpdateAudio")]
        [ProducesResponseType(typeof(AudioDetailViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Update(string audioId, [FromBody] UpdateAudioRequest request, 
            CancellationToken cancellationToken)
        {
            var result = await _audioService.Update(audioId, request, cancellationToken);
            return result.IsSuccess ? Ok(result.Data) : result.ReturnErrorResponse();
        }

        [HttpDelete("{audioId}", Name="DeleteAudio")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Destroy(string audioId, CancellationToken cancellationToken)
        {
            var result = await _audioService.Remove(audioId, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ReturnErrorResponse();
        }
    }
}
