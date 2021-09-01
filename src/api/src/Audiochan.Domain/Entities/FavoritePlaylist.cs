namespace Audiochan.Domain.Entities
{
    public class FavoritePlaylist
    {
        public long PlaylistId { get; set; }
        public Playlist Playlist { get; set; } = null!;
        public long UserId { get; set; }
        public User User { get; set; } = null!;
    }
}