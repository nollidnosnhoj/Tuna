using System;
using System.Linq;
using System.Linq.Expressions;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Audios.GetAudioList;

namespace Audiochan.Core.Features.Audios
{
    public static class AudioMaps
    {
        public static Expression<Func<Audio, AudioDetailViewModel>> AudioToDetailFunc = audio =>
            new AudioDetailViewModel
            {
                Id = audio.Id,
                Title = audio.Title,
                Description = audio.Description ?? string.Empty,
                Visibility = audio.Visibility,
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

        public static Expression<Func<Audio, AudioViewModel>> AudioToItemFunc = audio =>
            new AudioViewModel
            {
                Id = audio.Id,
                Title = audio.Title,
                Visibility = audio.Visibility,
                Duration = audio.Duration,
                Picture = audio.Picture != null
                    ? string.Format(MediaLinkInvariants.AudioPictureUrl, audio.Picture)
                    : null,
                Created = audio.Created,
                AudioUrl = string.Format(MediaLinkInvariants.AudioUrl, audio.File),
                User = new MetaAuthorDto(audio.User)
            };
    }
}