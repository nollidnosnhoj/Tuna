using System;
using System.Linq;
using System.Linq.Expressions;
using Audiochan.Core.Common;
using Audiochan.Core.Common.Helpers;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.GetAudio;

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
                Visibility = audio.Visibility,
                Secret = audio.Secret,
                Created = audio.Created,
                LastModified = audio.LastModified,
                Duration = audio.Duration,
                Size = audio.Size,
                Picture = audio.Picture != null
                    ? string.Format(MediaLinkInvariants.AudioPictureUrl, audio.Picture)
                    : null,
                Tags = audio.Tags.Select(t => t.Name).ToList(),
                AudioUrl = string.Format(MediaLinkInvariants.AudioUrl, audio.File),
                User = new MetaAuthorDto
                {
                    Id = audio.User.Id,
                    Username = audio.User.UserName,
                    Picture = audio.User.Picture != null
                    ? string.Format(MediaLinkInvariants.UserPictureUrl, audio.User.Picture)
                        : null
                },
                IsFavorited = userId > 0
                    ? audio.Favorited.Any(fa => fa.Id == userId)
                    : null
            };
        }
    }
}