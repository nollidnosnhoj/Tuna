namespace Audiochan.Core.Common.Models
{
    public record MetaAuthorDto
    {
        public string Id { get; init; } = null!;
        public string UserName { get; init; } = null!;
        public string? Picture { get; init; }
    }
}