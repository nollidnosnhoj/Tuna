using System;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Users.Models;

namespace Audiochan.Core.Features.Audios.Models
{
    public record AudioListViewModel
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public bool IsPublic { get; set; }
        public int FavoriteCount { get; set; }
        public bool IsFavorited { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public UserViewModel User { get; set; } = null!;

        public static AudioListViewModel From(Audio audio, long currentUserId)
        {
            return MapProjections.AudioList(currentUserId).Compile().Invoke(audio);
        }
    }
}