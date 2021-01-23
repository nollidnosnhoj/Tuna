using System;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Genres.Models;
using Audiochan.Core.Features.Users.Models;

namespace Audiochan.Core.Features.Audios.Models
{
    public record AudioDetailViewModel
    {
        public string Id { get; init; } = null!;
        public string Title { get; init; } = null!;
        public string Description { get; init; } = string.Empty;
        public bool IsPublic { get; init; }
        public bool IsLoop { get; init; }
        public string[] Tags { get; init; } = null!;
        public int Duration { get; init; }
        public long FileSize { get; init; }
        public string FileExt { get; init; } = null!;
        public string Url { get; init; } = null!;
        public string? ArtworkUrl { get; init; }
        public int FavoriteCount { get; init; }
        public bool IsFavorited { get; init; }
        public DateTime Created { get; init; }
        public DateTime? Updated { get; init; }
        public GenreDto Genre { get; init; } = null!;
        public UserViewModel User { get; init; } = null!;

        public static AudioDetailViewModel From(Audio audio, string currentUserId)
        {
            return MapProjections.AudioDetail(currentUserId).Compile().Invoke(audio);
        }
    }
}