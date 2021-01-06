using System;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Users.Models;

namespace Audiochan.Core.Features.Audios.Models
{
    public record AudioListViewModel
    {
        public string Id { get; init; } = null!;
        public string Title { get; init; } = null!;
        public bool IsPublic { get; init; }
        public int FavoriteCount { get; init; }
        public bool IsFavorited { get; init; }
        public DateTime Created { get; init; }
        public DateTime? Updated { get; init; }
        public UserViewModel User { get; init; } = null!;

        public static AudioListViewModel From(Audio audio, long currentUserId)
        {
            return MapProjections.AudioList(currentUserId).Compile().Invoke(audio);
        }
    }
}