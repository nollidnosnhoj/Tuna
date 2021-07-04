using System;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Interfaces;

namespace Audiochan.Core.Features.Playlists.GetPlaylistDetail
{
    public class GetPlaylistDetailCacheOptions : ICacheOptions
    {
        public GetPlaylistDetailCacheOptions(Guid id)
        {
            Key = CacheKeys.Playlist.GetPlaylist(id);
        }
        
        public string Key { get; }
        public TimeSpan Expiration => TimeSpan.FromMinutes(30);
    }
}