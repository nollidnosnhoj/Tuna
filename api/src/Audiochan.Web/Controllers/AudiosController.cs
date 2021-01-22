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
        [ProducesResponseType(typeof(PagedList<AudioListViewModel>), StatusCodes.Status200OK)]
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

        [HttpGet("random")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRandom(CancellationToken cancellationToken)
        {
            var result = await _audioService.GetRandom(cancellationToken);
            return result.IsSuccess 
                ? Ok(result.Data) 
                : result.ReturnErrorResponse();
        }

        [HttpPost(Name="UploadAudio")]
        [ProducesResponseType(typeof(AudioDetailViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Upload(
            [FromForm] UploadAudioRequest request
            , CancellationToken cancellationToken)
        {
            var result = await _audioService.Create(request, cancellationToken);
            return result.IsSuccess 
                ? CreatedAtAction(nameof(GetById), new { audioId = result.Data.Id }, result.Data)
                : result.ReturnErrorResponse();
        }

        [HttpPatch("{audioId}", Name="UpdateAudio")]
        [ProducesResponseType(typeof(AudioDetailViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
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
