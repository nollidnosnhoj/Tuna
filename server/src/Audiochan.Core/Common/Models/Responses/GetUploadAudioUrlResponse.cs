namespace Audiochan.Core.Common.Models.Responses
{
    public record GetUploadAudioUrlResponse
    {
        public string UploadId { get; init; }
        public string Url { get; init; }
    }
}