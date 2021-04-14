namespace Audiochan.API.Features.Audios.GetUploadAudioUrl
{
    public record GetUploadAudioUrlResponse
    {
        public string UploadId { get; init; }
        public string Url { get; init; }
    }
}