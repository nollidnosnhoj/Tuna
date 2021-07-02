using System;
using System.Collections.Generic;
using Audiochan.Core.Entities.Abstractions;
using Audiochan.Core.Entities.Enums;

namespace Audiochan.Core.Entities
{
    public class Playlist : IHasVisibility, IAudited
    {
        public Playlist()
        {
            Audios = new HashSet<PlaylistAudio>();
            Favorited = new HashSet<FavoritePlaylist>();
        }
        
        public Guid Id { get; set; }
        
        public string Title { get; set; } = null!;
        
        public string? Description { get; set; }
        
        public Visibility Visibility { get; set; }
        
        public string? Picture { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }

        public string UserId { get; set; } = null!;
        
        public User User { get; set; } = null!;
        
        public ICollection<PlaylistAudio> Audios { get; set; }
        public ICollection<FavoritePlaylist> Favorited { get; set; }
    }
}