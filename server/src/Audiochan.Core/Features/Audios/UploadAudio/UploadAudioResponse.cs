namespace Audiochan.Core.Features.Audios.UploadAudio
{
    public record UploadAudioResponse
    {
        public string UploadId { get; init; }
        public string UploadUrl { get; init; }
    }
}