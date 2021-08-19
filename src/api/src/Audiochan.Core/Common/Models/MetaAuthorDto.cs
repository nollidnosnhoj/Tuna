namespace Audiochan.Core.Common.Models
{
    public record MetaAuthorDto
    {
        public long Id { get; init; }
        public string Username { get; init; } = null!;
        public string? Picture { get; init; }
    }
}