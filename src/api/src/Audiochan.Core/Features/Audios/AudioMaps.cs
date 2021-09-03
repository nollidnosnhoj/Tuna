using System;
using System.Linq;
using System.Linq.Expressions;
using Audiochan.Core.Common.Models;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Features.Audios
{
    public static class AudioMaps
    {
        public static Expression<Func<Audio, AudioDto>> AudioToView(long? userId = null)
        {
            return audio => new AudioDto
            {
                Id = audio.Id,
                Title = audio.Title,
                Description = audio.Description ?? string.Empty,
                Created = audio.Created,
                LastModified = audio.LastModified,
                Duration = audio.Duration,
                Size = audio.Size,
                Picture = audio.Picture,
                Tags = audio.Tags.Select(t => t.Name).ToList(),
                Audio = audio.File,
                User = new MetaAuthorDto
                {
                    Id = audio.User.Id,
                    Username = audio.User.UserName,
                    Picture = audio.User.Picture
                },
                IsFavorited = userId > 0
                    ? audio.FavoriteAudios.Any(fa => fa.UserId == userId)
                    : null
            };
        }
    }
}