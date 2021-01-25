using System;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Users.Models;

namespace Audiochan.Core.Features.Audios.Models
{
    public record AudioListViewModel
    {
        public string Id { get; init; }
        public string Title { get; init; }
        public bool IsPublic { get; init; }
        public bool IsLoop { get; init; }
        public int FavoriteCount { get; init; }
        public bool IsFavorited { get; init; }
        public string PictureUrl { get; init; }
        public DateTime Created { get; init; }
        public DateTime? Updated { get; init; }
        public string Genre { get; init; }
        public UserViewModel User { get; init; }

        public static AudioListViewModel From(Audio audio, string currentUserId)
        {
            return MapProjections.AudioList(currentUserId).Compile().Invoke(audio);
        }
    }
}