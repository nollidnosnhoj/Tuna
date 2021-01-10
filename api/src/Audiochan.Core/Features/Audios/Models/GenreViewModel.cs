namespace Audiochan.Core.Features.Audios.Models
{
    public record GenreViewModel
    {
        public long Id { get; init; }
        public string Name { get; init; } = null!;
        public string Slug { get; init; } = null!;
    }
}