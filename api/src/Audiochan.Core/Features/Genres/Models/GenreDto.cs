namespace Audiochan.Core.Features.Genres.Models
{
    public record GenreDto
    {
        public long Id { get; init; }
        public string Name { get; init; }
        public string Slug { get; init; }
    }
}