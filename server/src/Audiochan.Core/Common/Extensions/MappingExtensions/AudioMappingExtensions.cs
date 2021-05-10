using System;
using System.Linq;
using System.Linq.Expressions;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios;

namespace Audiochan.Core.Common.Extensions.MappingExtensions
{
    public static class AudioMappingExtensions
    {
        private static Expression<Func<Audio, AudioDetailViewModel>> AudioToDetailProjection(
            MediaStorageSettings options)
        {
            return audio => new AudioDetailViewModel
            {
                Id = audio.Id,
                Title = audio.Title,
                Description = audio.Description,
                Duration = audio.Duration,
                Picture = audio.Picture,
                Uploaded = audio.Created,
                Tags = audio.Tags.Select(t => t.Name).ToArray(),
                IsPublic = audio.IsPublic,
                FileExt = audio.FileExt,
                FileSize = audio.FileSize,
                LastModified = audio.LastModified,
                AudioUrl = $"https://{options.Audio.Bucket}.s3.amazonaws.com/{options.Audio.Container}/{audio.FileName}",
                Author = new MetaAuthorDto
                {
                    Id = audio.User.Id,
                    Picture = $"https://{options.Image.Bucket}.s3.amazonaws.com/{options.Audio.Container}/${audio.User.Picture}",
                    Username = audio.User.UserName
                }
            };
        }

        private static Expression<Func<Audio, AudioViewModel>> AudioToListProjection(MediaStorageSettings options)
        {
            return audio => new AudioViewModel
            {
                Id = audio.Id,
                Title = audio.Title,
                Duration = audio.Duration,
                Picture = audio.Picture,
                Uploaded = audio.Created,
                IsPublic = audio.IsPublic,
                AudioUrl =
                    $"https://{options.Audio.Bucket}.s3.amazonaws.com/{options.Audio.Container}/{audio.FileName}",
                Author = new MetaAuthorDto
                {
                    Id = audio.User.Id,
                    Picture = $"https://{options.Image.Bucket}.s3.amazonaws.com/{options.Audio.Container}/${audio.User.Picture}",
                    Username = audio.User.UserName
                }
            };
        }

        public static IQueryable<AudioDetailViewModel> ProjectToDetail(this IQueryable<Audio> queryable,
            MediaStorageSettings options) => queryable.Select(AudioToDetailProjection(options));

        public static IQueryable<AudioViewModel> ProjectToList(this IQueryable<Audio> queryable,
            MediaStorageSettings options) =>
            queryable.Select(AudioToListProjection(options));

        public static AudioDetailViewModel MapToDetail(this Audio audio, MediaStorageSettings options) =>
            AudioToDetailProjection(options).Compile().Invoke(audio);
    }
}