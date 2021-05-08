namespace Audiochan.Core.Features.Audios.UploadAudio
{
    public record UploadAudioResponse
    {
        public string AudioId { get; init; }
        public string UploadUrl { get; init; }
    }
}