using System;
using System.Linq;
using System.Linq.Expressions;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Audios.GetAudioList;
using Audiochan.Core.Features.Shared.Responses;
using FastExpressionCompiler;

namespace Audiochan.Core.Common.Mappings
{
    public static class AudioMappings
    {
        public static IQueryable<AudioDetailViewModel> ProjectToDetail(this IQueryable<Audio> queryable) => 
            queryable.Select(AudioToDetailProjection());

        public static IQueryable<AudioViewModel> ProjectToList(this IQueryable<Audio> queryable) =>
            queryable.Select(AudioToListProjection());
        
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
                Picture = audio.PictureBlobName != null 
                    ? string.Format(MediaLinkInvariants.AudioPictureUrl, audio.PictureBlobName)
                    : null,
                Created = audio.Created,
                Tags = audio.Tags.Select(t => t.Name).ToList(),
                IsPublic = audio.IsPublic,
                FileExt = audio.FileExt,
                FileSize = audio.FileSize,
                LastModified = audio.LastModified,
                AudioUrl = string.Format(MediaLinkInvariants.AudioUrl, audio.Id, audio.BlobName),
                Author = new MetaAuthorDto
                {
                    Id = audio.User.Id,
                    Picture = audio.User.PictureBlobName != null
                        ? string.Format(MediaLinkInvariants.UserPictureUrl, audio.User.PictureBlobName)
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
                Picture = audio.PictureBlobName != null 
                    ? string.Format(MediaLinkInvariants.AudioPictureUrl, audio.PictureBlobName)
                    : null,
                Uploaded = audio.Created,
                IsPublic = audio.IsPublic,
                AudioUrl = string.Format(MediaLinkInvariants.AudioUrl, audio.Id, audio.BlobName),
                Author = new MetaAuthorDto
                {
                    Id = audio.User.Id,
                    Picture = audio.User.PictureBlobName != null
                        ? string.Format(MediaLinkInvariants.UserPictureUrl, audio.User.PictureBlobName)
                        : null,
                    Username = audio.User.UserName
                }
            };
        }
    }
}