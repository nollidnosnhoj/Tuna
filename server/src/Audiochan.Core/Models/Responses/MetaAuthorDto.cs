namespace Audiochan.Core.Models.Responses
{
    public record MetaAuthorDto
    {
        public string Id { get; init; }
        public string Username { get; init; }
        public string Picture { get; init; }
    }
}