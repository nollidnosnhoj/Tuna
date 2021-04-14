namespace Audiochan.API.Features.Upload.GetUploadAudioUrl
{
    public record GetUploadAudioUrlResponse
    {
        public string UploadId { get; init; }
        public string Url { get; init; }
    }
}