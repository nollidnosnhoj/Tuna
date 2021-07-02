using System;

namespace Audiochan.Core.Entities
{
    public class PlaylistAudio
    {
        public Guid PlaylistId { get; set; }
        public Playlist Playlist { get; set; } = null!;
        public Guid AudioId { get; set; }
        public Audio Audio { get; set; } = null!;
        public DateTime Added { get; set; }
    }
}