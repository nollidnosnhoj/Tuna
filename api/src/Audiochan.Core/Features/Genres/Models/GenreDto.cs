namespace Audiochan.Core.Features.Genres.Models
{
    public record GenreDto
    {
        public long Id { get; init; }
        public string Name { get; init; } = null!;
        public string Slug { get; init; } = null!;
    }
}