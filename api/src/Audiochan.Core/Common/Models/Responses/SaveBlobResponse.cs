namespace Audiochan.Core.Common.Models.Responses
{
    public record SaveBlobResponse
    {
        public string Bucket { get; init; } = null!;
        public string Url { get; init; } = null!;
        public string Path { get; init; } = null!;
        public string ContentType { get; init; } = null!;
    }
}