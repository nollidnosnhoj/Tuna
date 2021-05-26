using System;
using System.Linq;
using System.Linq.Expressions;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Audios.GetAudioList;
using FastExpressionCompiler;

namespace Audiochan.Core.Common.Extensions.MappingExtensions
{
    public static class AudioMappingExtensions
    {
        public static AudioDetailViewModel MapToDetail(this Audio audio, bool returnNullIfFail = false) =>
            AudioToDetailProjection().CompileFast(returnNullIfFail).Invoke(audio);
        
        public static Expression<Func<Audio, AudioDetailViewModel>> AudioToDetailProjection()
        {
            return audio => new AudioDetailViewModel
            {
                Id = audio.Id,
                Title = audio.Title,
                Description = audio.Description,
                Duration = audio.Duration,
                Picture = audio.Picture != null 
                    ? string.Format(MediaLinkInvariants.AudioPictureUrl, audio.Picture)
                    : null,
                Created = audio.Created,
                Tags = audio.Tags.Select(t => t.Name).ToList(),
                IsPublic = audio.IsPublic,
                FileExt = audio.FileExt,
                FileSize = audio.FileSize,
                LastModified = audio.LastModified,
                AudioUrl = string.Format(MediaLinkInvariants.AudioUrl, audio.Id, audio.FileName),
                Author = new MetaAuthorDto
                {
                    Id = audio.User.Id,
                    Picture = audio.User.Picture != null
                        ? string.Format(MediaLinkInvariants.UserPictureUrl, audio.User.Picture)
                        : null,
                    Username = audio.User.UserName
                }
            };
        }

        public static Expression<Func<Audio, AudioViewModel>> AudioToListProjection()
        {
            return audio => new AudioViewModel
            {
                Id = audio.Id,
                Title = audio.Title,
                Duration = audio.Duration,
                Picture = audio.Picture != null 
                    ? string.Format(MediaLinkInvariants.AudioPictureUrl, audio.Picture)
                    : null,
                Uploaded = audio.Created,
                IsPublic = audio.IsPublic,
                AudioUrl = string.Format(MediaLinkInvariants.AudioUrl, audio.Id, audio.FileName),
                Author = new MetaAuthorDto
                {
                    Id = audio.User.Id,
                    Picture = audio.User.Picture != null
                        ? string.Format(MediaLinkInvariants.UserPictureUrl, audio.User.Picture)
                        : null,
                    Username = audio.User.UserName
                }
            };
        }
    }
}