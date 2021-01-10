namespace Audiochan.Core.Entities
{
    public class Genre
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string Slug { get; set; } = null!;
    }
}