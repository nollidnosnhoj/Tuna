namespace Tuna.Domain.Entities;

public class FavoriteAudio
{
    public long AudioId { get; set; }
    public Audio Audio { get; set; } = null!;
    public long UserId { get; set; }
    public User User { get; set; } = null!;

    public DateTime Favorited { get; set; }
}