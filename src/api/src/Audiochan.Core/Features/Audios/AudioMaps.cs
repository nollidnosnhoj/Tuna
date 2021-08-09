using System;
using System.Linq;
using System.Linq.Expressions;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.GetAudio;

namespace Audiochan.Core.Features.Audios
{
    public static class AudioMaps
    {
        public static readonly Expression<Func<Audio, AudioViewModel>> AudioToView = audio =>
            new AudioViewModel
            {
                Id = audio.Id,
                Title = audio.Title,
                Slug = audio.FullSlug,
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
                User = new MetaAuthorDto(audio.User)
            };
    }
}