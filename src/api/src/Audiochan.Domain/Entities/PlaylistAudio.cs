namespace Audiochan.Domain.Entities
{
    public class PlaylistAudio
    {
        public long Id { get; set; }
        public long PlaylistId { get; set; }
        public Playlist Playlist { get; set; } = null!;
        public long AudioId { get; set; }
        public Audio Audio { get; set; } = null!;
    }
}