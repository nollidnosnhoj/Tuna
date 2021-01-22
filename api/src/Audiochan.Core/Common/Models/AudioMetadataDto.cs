namespace Audiochan.Core.Common.Models
{
    /// <summary>
    /// Contains metadata from an audio file.
    /// </summary>
    public record AudioMetadataDto(string? Title, int Duration)
    {
    }
}