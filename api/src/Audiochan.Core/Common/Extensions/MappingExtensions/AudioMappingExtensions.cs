using System;
using System.Linq;
using System.Linq.Expressions;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios;
using FastExpressionCompiler;

namespace Audiochan.Core.Common.Extensions.MappingExtensions
{
    public static class AudioMappingExtensions
    {
        public static IQueryable<AudioDetailViewModel> ProjectToDetail(this IQueryable<Audio> queryable,
            MediaStorageSettings options) => queryable.Select(AudioToDetailProjection(options));

        public static IQueryable<AudioViewModel> ProjectToList(this IQueryable<Audio> queryable,
            MediaStorageSettings options) =>
            queryable.Select(AudioToListProjection(options));

        public static AudioDetailViewModel MapToDetail(this Audio audio, MediaStorageSettings options, bool returnNullIfFail = false) =>
            AudioToDetailProjection(options).CompileFast(returnNullIfFail).Invoke(audio);
        
        private static Expression<Func<Audio, AudioDetailViewModel>> AudioToDetailProjection(
            MediaStorageSettings options)
        {
            return audio => new AudioDetailViewModel
            {
                Id = audio.Id,
                Title = audio.Title,
                Description = audio.Description,
                Duration = audio.Duration,
                Picture = audio.Picture != null 
                    ? $"https://{options.Image.Bucket}.s3.amazonaws.com/{options.Image.Container}/audios/{audio.Picture}"
                    : null,
                Created = audio.Created,
                Tags = audio.Tags.Select(t => t.Name).ToList(),
                IsPublic = audio.IsPublic,
                FileExt = audio.FileExt,
                FileSize = audio.FileSize,
                LastModified = audio.LastModified,
                AudioUrl = $"https://{options.Audio.Bucket}.s3.amazonaws.com/{options.Audio.Container}/{audio.Id}/{audio.FileName}",
                Author = new MetaAuthorDto
                {
                    Id = audio.User.Id,
                    Picture = audio.User.Picture != null
                        ? $"https://{options.Image.Bucket}.s3.amazonaws.com/{options.Audio.Container}/users/{audio.User.Picture}"
                        : null,
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
                Picture = audio.Picture != null 
                    ? $"https://{options.Image.Bucket}.s3.amazonaws.com/{options.Image.Container}/audios/{audio.Picture}"
                    : null,
                Uploaded = audio.Created,
                IsPublic = audio.IsPublic,
                AudioUrl =
                    $"https://{options.Audio.Bucket}.s3.amazonaws.com/{options.Audio.Container}/{audio.Id}/{audio.FileName}",
                Author = new MetaAuthorDto
                {
                    Id = audio.User.Id,
                    Picture = audio.User.Picture != null
                        ? $"https://{options.Image.Bucket}.s3.amazonaws.com/{options.Audio.Container}/users/{audio.User.Picture}"
                        : null,
                    Username = audio.User.UserName
                }
            };
        }
    }
}