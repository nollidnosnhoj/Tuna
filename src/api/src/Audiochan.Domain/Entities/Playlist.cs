using System;
using System.Collections.Generic;
using Audiochan.Domain.Abstractions;

namespace Audiochan.Domain.Entities
{
    public class Playlist : IAudited, IResourceEntity<long>
    {
        public Playlist()
        {
            Audios = new List<Audio>();
            Favorited = new HashSet<User>();
            PlaylistAudios = new HashSet<PlaylistAudio>();
            Tags = new HashSet<Tag>();
        }
        
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public string Slug { get; set; } = null!;
        public string? Description { get; set; }
        public string? Picture { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
        public long UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<Audio> Audios { get; set; }
        public ICollection<User> Favorited { get; set; }
        public ICollection<PlaylistAudio> PlaylistAudios { get; set; }
        public ICollection<Tag> Tags { get; set; }
    }
}