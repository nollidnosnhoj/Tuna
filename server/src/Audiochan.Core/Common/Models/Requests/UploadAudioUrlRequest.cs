namespace Audiochan.Core.Common.Models.Requests
{
    public record UploadAudioUrlRequest
    {
        public string FileName { get; init; } = null!;
        public long FileSize { get; init; }
    }
}