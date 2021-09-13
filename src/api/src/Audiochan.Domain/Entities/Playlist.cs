using System;
using System.Collections.Generic;
using System.Linq;
using Audiochan.Domain.Abstractions;

namespace Audiochan.Domain.Entities
{
    public class Playlist : IAudited, IHasId<long>
    {
        public Playlist()
        {
            Audios = new List<Audio>();
            FavoritePlaylists = new HashSet<FavoritePlaylist>();
            PlaylistAudios = new HashSet<PlaylistAudio>();
            Tags = new HashSet<Tag>();
        }
        
        public long Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public string? Picture { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
        public long UserId { get; set; }
        public User User { get; set; } = null!;

        public ICollection<Audio> Audios { get; set; }
        public ICollection<FavoritePlaylist> FavoritePlaylists { get; set; }
        public ICollection<PlaylistAudio> PlaylistAudios { get; set; }
        public ICollection<Tag> Tags { get; set; }

        public void AddAudios(IEnumerable<long> audioIds, DateTime addedDate)
        {
            foreach (var audioId in audioIds)
            {
                this.PlaylistAudios.Add(new PlaylistAudio
                {
                    PlaylistId = this.Id,
                    AudioId = audioId,
                    AddedBy = addedDate,
                });
            }
        }

        public void RemoveAudios(IEnumerable<long> audioIds)
        {
            foreach (var id in audioIds)
            {
                var playlistAudio = this.PlaylistAudios.FirstOrDefault(x => x.Id == id);
                if (playlistAudio is not null)
                    this.PlaylistAudios.Remove(playlistAudio);
            }
        }

        public void Favorite(long userId, DateTime favoritedDateTime)
        {
            var favoritePlaylist = this.FavoritePlaylists.FirstOrDefault(f => f.UserId == userId);

            if (favoritePlaylist is null)
            {
                this.FavoritePlaylists.Add(new FavoritePlaylist
                {
                    UserId = userId,
                    PlaylistId = this.Id,
                    Favorited = favoritedDateTime
                });
            }
        }

        public void UnFavorite(long userId)
        {
            var favoritePlaylist = this.FavoritePlaylists.FirstOrDefault(f => f.UserId == userId);

            if (favoritePlaylist is not null)
            {
                this.FavoritePlaylists.Remove(favoritePlaylist);
            }
        }
    }
}