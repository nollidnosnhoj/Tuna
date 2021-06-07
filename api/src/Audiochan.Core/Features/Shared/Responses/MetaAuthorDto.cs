namespace Audiochan.Core.Features.Shared.Responses
{
    public record MetaAuthorDto
    {
        public string Id { get; init; } = null!;
        public string Username { get; init; } = null!;
        public string? Picture { get; init; }
    }
}