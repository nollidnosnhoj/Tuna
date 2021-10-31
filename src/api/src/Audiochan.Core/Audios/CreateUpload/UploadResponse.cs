namespace Audiochan.Core.Audios
{
    public record UploadResponse
    {
        public string UploadId { get; init; } = null!;
        public string UploadUrl { get; init; } = null!;
    }
}