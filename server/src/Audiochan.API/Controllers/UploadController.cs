using System.Collections.Generic;
using System.IO;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class UploadController : ControllerBase
    {
        private readonly MediaStorageSettings _storageSettings;
        private readonly ICurrentUserService _currentUserService;
        private readonly IStorageService _storageService;

        public UploadController(MediaStorageSettings storageSettings, ICurrentUserService currentUserService, IStorageService storageService)
        {
            _storageSettings = storageSettings;
            _currentUserService = currentUserService;
            _storageService = storageService;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Get a temporary pre-signed AWS S3 Put link to upload audio.",
            OperationId = "GetPresignedUrl",
            Tags = new[] {"upload"}
        )]
        public IActionResult GetUploadUrl([FromBody] UploadAudioUrlRequest request)
        {
            var userId = _currentUserService.GetUserId();
            var uploadId = AudioHelpers.GenerateUploadId();
            var blobName = uploadId + Path.GetExtension(request.FileName);
            var metadata = new Dictionary<string, string> {{"UserId", userId}, {"OriginalFilename", request.FileName}};
            var presignedUrl = _storageService.GetPresignedUrl(
                method: "put",
                bucket: _storageSettings.Audio.Bucket,
                container: _storageSettings.Audio.Container,
                blobName: blobName,
                expirationInMinutes: 5,
                metadata: metadata);
            var response = new GetUploadAudioUrlResponse {Url = presignedUrl, UploadId = uploadId};
            return new JsonResult(response);
        }
    }
}