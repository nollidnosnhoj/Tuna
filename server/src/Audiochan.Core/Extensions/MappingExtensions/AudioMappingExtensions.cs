using System;
using System.Linq;
using System.Linq.Expressions;
using Audiochan.Core.Entities;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Enums;
using Audiochan.Core.Models.Responses;
using Audiochan.Core.Models.ViewModels;
using Audiochan.Core.Settings;

namespace Audiochan.Core.Extensions.MappingExtensions
{
    public static class AudioMappingExtensions
    {
        private static Expression<Func<Audio, AudioDetailViewModel>> AudioToDetailProjection(MediaStorageSettings options)
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
                Visibility = audio.Visibility,
                FileExt = audio.FileExt,
                FileSize = audio.FileSize,
                LastModified = audio.LastModified,
                PrivateKey = audio.Visibility == Visibility.Private ? audio.PrivateKey : null,
                AudioUrl = $"{options.StorageUrl}/{options.Audio.Container}/{audio.UploadId + audio.FileExt}",
                Author = new MetaAuthorDto
                {
                    Id = audio.User.Id,
                    Picture = audio.User.Picture,
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
                Visibility = audio.Visibility,
                AudioUrl = $"{options.StorageUrl}/{options.Audio.Container}/{audio.UploadId + audio.FileExt}",
                Author = new MetaAuthorDto
                {
                    Id = audio.User.Id,
                    Picture = audio.User.Picture,
                    Username = audio.User.UserName
                }
            };
        }

        public static IQueryable<AudioDetailViewModel> ProjectToDetail(this IQueryable<Audio> queryable,
            MediaStorageSettings options) => queryable.Select(AudioToDetailProjection(options));

        public static IQueryable<AudioViewModel> ProjectToList(this IQueryable<Audio> queryable, MediaStorageSettings options) =>
            queryable.Select(AudioToListProjection(options));

        public static AudioDetailViewModel MapToDetail(this Audio audio, MediaStorageSettings options) =>
            AudioToDetailProjection(options).Compile().Invoke(audio);
    }
}