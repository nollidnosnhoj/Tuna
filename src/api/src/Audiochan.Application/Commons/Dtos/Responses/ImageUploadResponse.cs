namespace Audiochan.Application.Commons.Dtos.Responses
{
    public record ImageUploadResponse
    {
        public string Url { get; init; } = null!;
    }
}