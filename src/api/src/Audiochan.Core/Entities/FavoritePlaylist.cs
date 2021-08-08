using System;
using Audiochan.Core.Entities.Abstractions;

namespace Audiochan.Core.Entities
{
    public class FavoritePlaylist
    {
        public Guid PlaylistId { get; set; }
        public Playlist Playlist { get; set; } = null!;
        public long UserId { get; set; }
        public User User { get; set; } = null!;
    }
}