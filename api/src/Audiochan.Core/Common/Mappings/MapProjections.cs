using System;
using System.Linq;
using System.Linq.Expressions;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Features.Followers.Models;
using Audiochan.Core.Features.Genres.Models;
using Audiochan.Core.Features.Users.Models;

namespace Audiochan.Core.Common.Mappings
{
    public static class MapProjections
    {
        public static Expression<Func<Audio, AudioDetailViewModel>> AudioDetail(string currentUserId)
        {
            return audio => new AudioDetailViewModel
            {
                Id = audio.Id,
                Title = audio.Title,
                Description = audio.Description,
                IsPublic = audio.IsPublic,
                IsLoop = audio.IsLoop,
                Duration = audio.Duration,
                FileSize = audio.AudioFileSize,
                FileExt = audio.AudioFileExtension,
                Url = audio.AudioUrl,
                PictureUrl = audio.PictureUrl,
                Tags = audio.Tags.Select(tag => tag.Id).ToArray(),
                FavoriteCount = audio.Favorited.Count,
                IsFavorited = currentUserId != null 
                              && currentUserId.Length > 0
                              && audio.Favorited.Any(f => f.UserId == currentUserId),
                Created = audio.Created,
                Updated = audio.LastModified,
                Genre = new GenreDto
                {
                    Id = audio.Genre.Id,
                    Name = audio.Genre.Name,
                    Slug = audio.Genre.Slug
                },
                User = new UserViewModel
                {
                    Id = audio.User.Id,
                    Username = audio.User.UserName,
                },
            };
        }

        public static Expression<Func<Audio, AudioListViewModel>> AudioList(string currentUserId)
        {
            return audio => new AudioListViewModel
            {
                Id = audio.Id,
                Title = audio.Title,
                IsPublic = audio.IsPublic,
                IsLoop = audio.IsLoop,
                Duration = audio.Duration,
                PictureUrl = audio.PictureUrl,
                FavoriteCount = audio.Favorited.Count,
                IsFavorited = currentUserId != null 
                              && currentUserId.Length > 0
                              && audio.Favorited.Any(f => f.UserId == currentUserId),
                Created = audio.Created,
                Updated = audio.LastModified,
                Genre = audio.Genre.Name,
                User = new UserViewModel
                {
                    Id = audio.User.Id,
                    Username = audio.User.UserName
                }
            };
        }

        public static Expression<Func<FollowedUser, FollowUserViewModel>> FollowUser(string currentUserId)
        {
            return u => new FollowUserViewModel
            {
                Id = u.Target.Id,
                Username = u.Target.UserName,
                AvatarUrl = u.Target.PictureUrl,
                IsFollowing = u.Target.Followers.Any(x => x.ObserverId == currentUserId)
            };
        }

        public static Expression<Func<User, UserDetailsViewModel>> UserDetails(string currentUserId)
        {
            return user => new UserDetailsViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                AboutMe = user.About,
                Website = user.Website,
                PictureUrl = user.PictureUrl,
                AudioCount = user.Audios.Count,
                FollowerCount = user.Followers.Count,
                FollowingCount = user.Followings.Count,
                IsFollowing = user.Followers.Any(f => f.ObserverId == currentUserId)
            };
        }

        public static Expression<Func<Genre, GenreDto>> Genre()
        {
            return genre => new GenreDto
            {
                Id = genre.Id,
                Name = genre.Name,
                Slug = genre.Slug
            };
        }
    }
}