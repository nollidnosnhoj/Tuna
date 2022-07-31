namespace Audiochan.Core.Dtos.Requests
{
    public record ImageUploadRequest
    {
        public string Data { get; init; } = string.Empty;
    }
}