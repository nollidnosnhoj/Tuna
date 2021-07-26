using System;

namespace Audiochan.Core.Common.Constants
{
    public static class CacheKeys
    {
        public static class Audio
        {
            public static string GetAudio(Guid audioId) => $"audio_id_{audioId}";
        }

        public static class Playlist
        {
            public static string GetPlaylist(Guid playlistId) => $"playlist_id_{playlistId}";
        }
    }
}