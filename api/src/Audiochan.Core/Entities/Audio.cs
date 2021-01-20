using System;
using System.Collections.Generic;
using Audiochan.Core.Entities.Base;

namespace Audiochan.Core.Entities
{
    public class Audio : BaseEntity
    {
        public Audio()
        {
            Tags = new HashSet<Tag>();
            Favorited = new HashSet<FavoriteAudio>();
        }

        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int Duration { get; set; }
        public long FileSize { get; set; }
        public string FileExt { get; set; } = null!;
        public string Url { get; set; } = null!;
        public bool IsPublic { get; set; }
        public bool IsLoop { get; set; }
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        public long GenreId { get; set; }
        public Genre Genre { get; set; } = null!;
        public ICollection<Tag> Tags { get; set; }
        public ICollection<FavoriteAudio> Favorited { get; }
    }
}
