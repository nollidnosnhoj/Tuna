namespace Audiochan.Core.Features.Upload.Dtos
{
    public record CreateUploadResponse
    {
        public string UploadId { get; init; } = null!;
        public string UploadUrl { get; init; } = null!;
    }
}