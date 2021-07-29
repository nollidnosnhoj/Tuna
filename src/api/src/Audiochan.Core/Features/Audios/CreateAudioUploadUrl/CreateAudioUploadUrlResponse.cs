namespace Audiochan.Core.Features.Audios.CreateAudioUploadUrl
{
    public record CreateAudioUploadUrlResponse
    {
        public string UploadId { get; init; } = null!;
        public string UploadUrl { get; init; } = null!;
    }
}