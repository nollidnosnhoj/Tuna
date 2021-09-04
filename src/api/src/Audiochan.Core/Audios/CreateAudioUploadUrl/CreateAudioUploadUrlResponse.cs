namespace Audiochan.Core.Audios.CreateAudioUploadUrl
{
    public record CreateAudioUploadUrlResponse
    {
        public string UploadId { get; init; } = null!;
        public string UploadUrl { get; init; } = null!;
    }
}