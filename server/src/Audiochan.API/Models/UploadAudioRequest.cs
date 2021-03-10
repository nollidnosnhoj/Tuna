namespace Audiochan.API.Models
{
    public record UploadAudioRequest
    {
        public string FileName { get; init; }
    }
}