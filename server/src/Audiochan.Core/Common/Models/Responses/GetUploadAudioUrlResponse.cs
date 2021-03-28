namespace Audiochan.Core.Common.Models.Responses
{
    public record GetUploadAudioUrlResponse : GetPresignedUrlResponse
    {
        public string UploadId { get; init; }
    }
}