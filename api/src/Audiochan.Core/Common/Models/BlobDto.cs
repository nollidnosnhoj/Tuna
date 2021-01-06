namespace Audiochan.Core.Common.Models
{
    public record BlobDto(bool FoundBlob, string Container, string Name, string Url, long Size) { }
}
