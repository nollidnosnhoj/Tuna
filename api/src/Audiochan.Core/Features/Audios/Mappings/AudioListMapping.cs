using System;
using System.Linq;
using System.Linq.Expressions;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Features.Users.Models;

namespace Audiochan.Core.Features.Audios.Mappings
{
    public static class AudioListMapping
    {
        public static Expression<Func<Audio, AudioListViewModel>> Map(string currentUserId)
        {
            return audio => new AudioListViewModel
            {
                Id = audio.Id,
                Title = audio.Title,
                IsPublic = audio.IsPublic,
                IsLoop = audio.IsLoop,
                Duration = audio.Duration,
                PictureUrl = audio.PictureUrl,
                FavoriteCount = audio.Favorited.Count,
                IsFavorited = currentUserId != null 
                              && currentUserId.Length > 0
                              && audio.Favorited.Any(f => f.UserId == currentUserId),
                Created = audio.Created,
                Updated = audio.LastModified,
                Genre = audio.Genre.Name,
                User = new UserViewModel
                {
                    Id = audio.User.Id,
                    Username = audio.User.UserName
                }
            };
        }
    }
}