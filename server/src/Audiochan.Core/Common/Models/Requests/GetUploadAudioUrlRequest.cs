namespace Audiochan.Core.Common.Models.Requests
{
    public record GetUploadAudioUrlRequest
    {
        public string FileName { get; init; }
        public long FileSize { get; init; }
    }
}