using System;
using System.Collections.Generic;
using Audiochan.Core.Entities.Base;

namespace Audiochan.Core.Entities
{
    public class Audio : BaseEntity
    {
        public Audio()
        {
            Tags = new HashSet<AudioTag>();
            Favorited = new HashSet<FavoriteAudio>();
        }

        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int Duration { get; set; }
        public long FileSize { get; set; }
        public string FileExt { get; set; } = null!;
        public bool IsUploaded { get; set; }
        public bool IsPublic { get; set; }
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<AudioTag> Tags { get; set; }
        public ICollection<FavoriteAudio> Favorited { get; }
    }
}
