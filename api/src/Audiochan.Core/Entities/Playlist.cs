using System;
using System.Collections.Generic;
using System.Linq;
using Audiochan.Core.Entities.Abstractions;
using Audiochan.Core.Entities.Enums;

namespace Audiochan.Core.Entities
{
    public class Playlist : IVisible, IAudited
    {
        public Playlist()
        {
            Audios = new HashSet<PlaylistAudio>();
            Favoriters = new HashSet<FavoritePlaylist>();
        }
        
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public Visibility Visibility { get; set; }
        public string? PrivateKey { get; set; }
        public string? Picture => this.Audios.FirstOrDefault()?.Audio.PictureBlobName;
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
        
        public string UserId { get; set; } = null!;
        public User User { get; set; } = null!;
        public ICollection<PlaylistAudio> Audios { get; set; }
        public ICollection<FavoritePlaylist> Favoriters { get; set; }
    }
}