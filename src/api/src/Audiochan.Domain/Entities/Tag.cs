using System.Collections.Generic;
using Audiochan.Domain.Abstractions;

namespace Audiochan.Domain.Entities
{
    public class Tag : IHasId<long>
    {
        public Tag()
        {
            Audios = new HashSet<Audio>();
            Playlists = new HashSet<Playlist>();
        }

        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public ICollection<Audio> Audios { get; set; }
        public ICollection<Playlist> Playlists { get; set; }
    }
}