using System;

namespace Audiochan.Core.Entities
{
    public class PlaylistAudio
    {
        public long PlaylistId { get; set; }
        public Playlist Playlist { get; set; } = null!;
        public long AudioId { get; set; }
        public Audio Audio { get; set; } = null!;
        public DateTime Added { get; set; }
    }
}