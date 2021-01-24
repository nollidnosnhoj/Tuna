using Microsoft.AspNetCore.Http;

namespace Audiochan.Core.Features.Audios.Models
{
    public record UploadAudioRequest : UpdateAudioRequest
    {
        public IFormFile File { get; init; } = null!;
        public IFormFile? Image { get; init; }
    }
}