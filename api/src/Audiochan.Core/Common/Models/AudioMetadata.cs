namespace Audiochan.Core.Common.Models
{
    /// <summary>
    /// Contains metadata from an audio file.
    /// </summary>
    public record AudioMetadata(string? Title, int Duration)
    {
    }
}