using System;
using Audiochan.Core.Common.Mappings;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Users.Models;

namespace Audiochan.Core.Features.Audios.Models
{
    public record AudioDetailViewModel
    {
        public string Id { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Description { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public string[] Tags { get; set; } = null!;
        public int Duration { get; set; }
        public long FileSize { get; set; }
        public string FileExt { get; set; } = null!;
        public string Url { get; set; } = null!;
        public int FavoriteCount { get; set; }
        public bool IsFavorited { get; set; }
        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }
        public UserViewModel User { get; set; } = null!;

        public static AudioDetailViewModel From(Audio audio, long currentUserId)
        {
            return MapProjections.AudioDetail(currentUserId).Compile().Invoke(audio);
        }
    }
}