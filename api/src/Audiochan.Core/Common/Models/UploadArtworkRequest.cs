using Microsoft.AspNetCore.Http;

namespace Audiochan.Core.Common.Models
{
    public record UploadArtworkRequest
    {
        public IFormFile? Image { get; init; }
    }
}