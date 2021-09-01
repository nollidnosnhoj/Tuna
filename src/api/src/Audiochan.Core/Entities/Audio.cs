using System;
using System.Collections.Generic;
using System.Linq;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Entities.Abstractions;
using Audiochan.Core.Entities.Enums;

namespace Audiochan.Core.Entities
{
    public class Audio : IAudited, IResourceEntity<long>
    {
        public Audio()
        {
            this.Favorited = new HashSet<User>();
            this.Tags = new HashSet<Tag>();
            this.Playlists = new List<Playlist>();
        }
        
        public long Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public decimal Duration { get; set; }
        public string File { get; set; } = null!;
        public long Size { get; set; }
        public string? Picture { get; set; }
        public long UserId { get; set; }
        public User User { get; set; } = null!;
        public ICollection<Playlist> Playlists { get; set; }
        public ICollection<User> Favorited { get; set; }
        public ICollection<Tag> Tags { get; set; }

        public void UpdateTags(List<Tag> tags)
        {
            if (this.Tags.Count > 0)
            {
                foreach (var audioTag in this.Tags.ToList())
                {
                    if (tags.All(t => t.Name != audioTag.Name))
                    {
                        this.Tags.Remove(audioTag);
                    }
                }

                foreach (var tag in tags)
                {
                    if (this.Tags.All(t => t.Name != tag.Name))
                        this.Tags.Add(tag);
                }
            }
            else
            {
                foreach (var tag in tags)
                {
                    this.Tags.Add(tag);
                }
            }
        }
    }
}