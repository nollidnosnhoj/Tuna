namespace Audiochan.Core.Features.Genres.ListGenre
{
    public record GenreViewModel
    {
        public long Id { get; init; }
        public string Name { get; init; }
        public string Slug { get; init; }
        public int? Count { get; init; }
    }
}