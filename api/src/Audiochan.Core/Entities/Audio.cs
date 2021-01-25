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

        public string Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public long AudioFileSize { get; set; }
        public string AudioFileExtension { get; set; }
        public string AudioUrl { get; set; }
        public string PictureUrl { get; set; }
        public bool IsPublic { get; set; }
        public bool IsLoop { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public long GenreId { get; set; }
        public Genre Genre { get; set; }
        public ICollection<Tag> Tags { get; set; }
        public ICollection<FavoriteAudio> Favorited { get; }
    }
}
