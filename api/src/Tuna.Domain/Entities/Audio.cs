using Tuna.Domain.Entities.Abstractions;

namespace Tuna.Domain.Entities;

public enum AudioStatus
{
    Draft,
    Published
}

public class Audio : AuditableEntity<long>
{
    private Audio()
    {
    }

    public Audio(string fileId, string fileName, long fileSize, long userId)
    {
        Title = Path.GetFileNameWithoutExtension(fileName);
        FileSize = fileSize;
        UserId = userId;
        FileId = fileId;
    }

    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Duration { get; set; }
    public string FileId { get; set; } = null!;
    public long FileSize { get; set; }
    public string? ImageId { get; set; }
    public AudioStatus Status { get; set; } = AudioStatus.Draft;
    public DateTime? PublishedAt { get; set; }
    public DateTime? UploadedAt { get; set; }
    public long UserId { get; set; }
    public User User { get; set; } = null!;
    public ICollection<FavoriteAudio> FavoriteAudios { get; set; } = new HashSet<FavoriteAudio>();

    public void Publish(DateTime publishedAt)
    {
        Status = AudioStatus.Published;
        PublishedAt = publishedAt;
    }

    public void CompleteUpload(DateTime uploadedAt)
    {
        UploadedAt = uploadedAt;
    }

    public void Favorite(long userId, DateTime favoritedDateTime)
    {
        var favoriteAudio = FavoriteAudios.FirstOrDefault(f => f.UserId == userId);

        if (favoriteAudio is null)
            FavoriteAudios.Add(new FavoriteAudio
            {
                UserId = userId,
                AudioId = Id,
                Favorited = favoritedDateTime
            });
    }

    public void UnFavorite(long userId)
    {
        var favoriteAudio = FavoriteAudios.FirstOrDefault(f => f.UserId == userId);

        if (favoriteAudio is not null) FavoriteAudios.Remove(favoriteAudio);
    }
}