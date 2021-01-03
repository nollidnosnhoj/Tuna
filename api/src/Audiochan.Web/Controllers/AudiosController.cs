﻿using System.Collections.Generic;
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
        private readonly IAudioFavoriteService _audioFavoriteService;
        private readonly ICurrentUserService _currentUserService;

        public AudiosController(IAudioService audioService, IAudioFavoriteService audioFavoriteService, 
            ICurrentUserService currentUserService)
        {
            _audioService = audioService;
            _audioFavoriteService = audioFavoriteService;
            _currentUserService = currentUserService;
        }

        [HttpGet(Name="GetAudios")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(List<AudioListViewModel>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetList([FromQuery] GetAudioListQuery query, 
            CancellationToken cancellationToken)
        {
            return Ok(await _audioService.GetList(query, cancellationToken));
        }

        [HttpGet("{audioId}", Name = "GetAudioById")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AudioDetailViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(string audioId, CancellationToken cancellationToken)
        {
            var result = await _audioService.Get(audioId, cancellationToken);

            return result.IsSuccess
                ? Ok(result.Data)
                : result.ReturnErrorResponse();
        }

        [HttpGet("random-id")]
        [AllowAnonymous]
        public async Task<IActionResult> GetRandomId(CancellationToken cancellationToken)
        {
            return Ok(await _audioService.GetRandomAudioId(cancellationToken));
        }

        [HttpPost(Name="UploadAudio")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AudioDetailViewModel), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Upload(
            [FromForm] UploadAudioRequest request
            , CancellationToken cancellationToken)
        {
            var uploadResult = await _audioService.Create(request, cancellationToken);

            if (uploadResult.IsSuccess)
            {
                return CreatedAtRoute(new {uploadResult.Data.Id}, uploadResult.Data);
            }
            
            return uploadResult.ReturnErrorResponse();
        }

        [HttpPatch("{audioId}", Name="UpdateAudio")]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AudioDetailViewModel), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> Update(string audioId, [FromBody] UpdateAudioRequest request, 
            CancellationToken cancellationToken)
        {
            var result = await _audioService.Update(audioId, request, cancellationToken);

            return result.IsSuccess
                ? Ok(result.Data)
                : result.ReturnErrorResponse();
        }

        [HttpDelete("{audioId}", Name="DeleteAudio")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Destroy(string audioId, CancellationToken cancellationToken)
        {
            var result = await _audioService.Remove(audioId, cancellationToken);

            return result.IsSuccess
                ? NoContent()
                : result.ReturnErrorResponse();
        }

        [HttpPost("{audioId}/favorites", Name = "FavoriteAudio")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Favorite(string audioId, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var result = await _audioFavoriteService.FavoriteAudio(currentUserId, audioId, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ReturnErrorResponse();
        }
        
        [HttpDelete("{audioId}/favorites", Name = "UnfavoriteAudio")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Unfavorite(string audioId, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();
            var result = await _audioFavoriteService.UnfavoriteAudio(currentUserId, audioId, cancellationToken);
            return result.IsSuccess ? NoContent() : result.ReturnErrorResponse();
        }

        [HttpHead("{audioId}/favorites", Name="IsFavorited")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ErrorViewModel), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CheckFavorite(string audioId, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.GetUserId();

            return await _audioFavoriteService.CheckIfUserFavorited(currentUserId, audioId, cancellationToken)
                ? NoContent()
                : NotFound();
        }
    }
}
