namespace Audiochan.Core.Audios.Commands
{
    public record UploadResponse
    {
        public string UploadId { get; init; } = null!;
        public string UploadUrl { get; init; } = null!;
    }
}