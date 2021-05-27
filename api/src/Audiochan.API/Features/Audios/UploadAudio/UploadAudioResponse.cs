namespace Audiochan.API.Features.Audios.UploadAudio
{
    public record UploadAudioResponse
    {
        public string UploadId { get; init; } = null!;
        public string UploadUrl { get; init; } = null!;
    }
}