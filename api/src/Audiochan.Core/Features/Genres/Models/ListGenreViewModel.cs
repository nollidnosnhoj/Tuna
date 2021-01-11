namespace Audiochan.Core.Features.Genres.Models
{
    public record ListGenreViewModel
    {
        public long Id { get; init; }
        public string Name { get; init; } = null!;
        public string Slug { get; init; } = null!;
        public int? Count { get; init; } = null;
    }
}