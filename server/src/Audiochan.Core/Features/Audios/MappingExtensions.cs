using System;
using System.Linq;
using System.Linq.Expressions;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Audios.GetAudioList;

namespace Audiochan.Core.Features.Audios
{
    public static class AudioMappingExtensions
    {
        public static Expression<Func<Audio, AudioDetailViewModel>> AudioToDetailProjection()
        {
            return audio => new AudioDetailViewModel
            {
                Id = audio.Id,
                Title = audio.Title,
                Description = audio.Description,
                Duration = audio.Duration,
                Picture = audio.Picture,
                Created = audio.Created,
                Tags = audio.Tags.Select(t => t.Id).ToArray(),
                Visibility = audio.Visibility,
                FileExt = audio.FileExt,
                FileSize = audio.FileSize,
                LastModified = audio.LastModified,
                PrivateKey = audio.Visibility == Visibility.Private ? audio.PrivateKey : null,
                User = new MetaUserDto(audio.User)
            };
        }

        public static Expression<Func<Audio, AudioViewModel>> AudioToListProjection()
        {
            return audio => new AudioViewModel
            {
                Id = audio.Id,
                Title = audio.Title,
                Duration = audio.Duration,
                Picture = audio.Picture,
                Created = audio.Created,
                Visibility = audio.Visibility,
                User = new MetaUserDto(audio.User)
            };
        }
    }
}