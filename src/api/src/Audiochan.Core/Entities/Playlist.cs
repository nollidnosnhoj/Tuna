using System;
using System.Collections.Generic;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Entities.Abstractions;
using Audiochan.Core.Entities.Enums;

namespace Audiochan.Core.Entities
{
    public class Playlist : IHasVisibility, IAudited, IResourceEntity<long>
    {
        public Playlist()
        {
            Audios = new List<PlaylistAudio>();
            Favorited = new HashSet<FavoritePlaylist>();
            Tags = new HashSet<Tag>();
        }
        
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Description { get; set; }
        public Visibility Visibility { get; set; }
        public string? Secret { get; set; }
        public string? Picture { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
        public long UserId { get; set; }
        public User User { get; set; } = null!;

        public string FullSlug => $"{Id}-{Slug}";
        
        public ICollection<PlaylistAudio> Audios { get; set; }
        public ICollection<FavoritePlaylist> Favorited { get; set; }
        public ICollection<Tag> Tags { get; set; }
    }
}