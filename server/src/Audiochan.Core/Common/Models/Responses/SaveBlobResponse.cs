namespace Audiochan.Core.Common.Models.Responses
{
    public record SaveBlobResponse
    {
        public string Bucket { get; init; }
        public string Url { get; init; }
        public string Path { get; init; }
        public string ContentType { get; init; }
    }
}