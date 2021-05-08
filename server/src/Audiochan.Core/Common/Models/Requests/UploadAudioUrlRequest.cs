namespace Audiochan.Core.Common.Models.Requests
{
    public record UploadAudioUrlRequest
    {
        public string FileName { get; init; }
        public long FileSize { get; init; }
        public int Duration { get; init; }
    }
}