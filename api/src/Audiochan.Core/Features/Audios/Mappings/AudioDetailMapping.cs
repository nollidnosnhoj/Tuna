using System;
using System.Linq;
using System.Linq.Expressions;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Features.Genres.Models;
using Audiochan.Core.Features.Users.Models;

namespace Audiochan.Core.Features.Audios.Mappings
{
    public static class AudioDetailMapping
    {
        public static Expression<Func<Audio, AudioDetailViewModel>> Map(string currentUserId)
        {
            return audio => new AudioDetailViewModel
            {
                Id = audio.Id,
                Title = audio.Title,
                Description = audio.Description,
                IsPublic = audio.IsPublic,
                IsLoop = audio.IsLoop,
                Duration = audio.Duration,
                FileSize = audio.AudioFileSize,
                FileExt = audio.AudioFileExtension,
                Url = audio.AudioUrl,
                PictureUrl = audio.PictureUrl,
                Tags = audio.Tags.Select(tag => tag.Id).ToArray(),
                FavoriteCount = audio.Favorited.Count,
                IsFavorited = currentUserId != null 
                              && currentUserId.Length > 0
                              && audio.Favorited.Any(f => f.UserId == currentUserId),
                Created = audio.Created,
                Updated = audio.LastModified,
                Genre = new GenreDto
                {
                    Id = audio.Genre.Id,
                    Name = audio.Genre.Name,
                    Slug = audio.Genre.Slug
                },
                User = new UserViewModel
                {
                    Id = audio.User.Id,
                    Username = audio.User.UserName,
                },
            };
        }

        public static AudioDetailViewModel MapToDetail(this Audio audio, string currentUserId) => 
            Map(currentUserId).Compile().Invoke(audio);
    }
}