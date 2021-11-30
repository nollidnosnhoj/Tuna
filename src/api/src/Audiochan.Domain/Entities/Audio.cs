using System;
using System.Collections.Generic;
using System.Linq;
using Audiochan.Domain.Abstractions;

namespace Audiochan.Domain.Entities
{
    public class Audio : IAudited, IHasId<long>
    {
        public long Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public List<string> Tags { get; set; } = new();
        public decimal Duration { get; set; }
        public string File { get; set; } = null!;
        public long Size { get; set; }
        public string? Picture { get; set; }
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<FavoriteAudio> FavoriteAudios { get; set; } = new HashSet<FavoriteAudio>();

        public void Favorite(long userId, DateTime favoritedDateTime)
        {
            var favoriteAudio = this.FavoriteAudios.FirstOrDefault(f => f.UserId == userId);

            if (favoriteAudio is null)
            {
                this.FavoriteAudios.Add(new FavoriteAudio
                {
                    UserId = userId,
                    AudioId = this.Id,
                    Favorited = favoritedDateTime
                });
            }
        }

        public void UnFavorite(long userId)
        {
            var favoriteAudio = this.FavoriteAudios.FirstOrDefault(f => f.UserId == userId);

            if (favoriteAudio is not null)
            {
                this.FavoriteAudios.Remove(favoriteAudio);
            }
        }
    }
}