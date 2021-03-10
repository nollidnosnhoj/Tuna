using Audiochan.API.Models;
using Audiochan.Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    public class UploadController : ControllerBase
    {
        private readonly IUploadService _uploadService;

        public UploadController(IUploadService uploadService)
        {
            _uploadService = uploadService;
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Get a temporary pre-signed AWS S3 Put link to upload audio.",
            OperationId = "GetPresignedUrl",
            Tags = new[] {"upload"}
        )]
        public IActionResult GetUploadUrl([FromBody] UploadAudioRequest request)
        {
            var (uploadId, uploadLink) = _uploadService.GetUploadUrl(request.FileName);
            return new JsonResult(new UploadAudioResponse(uploadId, uploadLink));
        }
    }
}