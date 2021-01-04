using System;
using System.Linq;
using System.Linq.Expressions;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Features.Followers.Models;
using Audiochan.Core.Features.Users.Models;

namespace Audiochan.Core.Common.Mappings
{
    public static class MapProjections
    {
        public static Expression<Func<Audio, AudioDetailViewModel>> AudioDetail(long currentUserId)
        {
            return audio => new AudioDetailViewModel
            {
                Id = audio.Id,
                Title = audio.Title,
                Description = audio.Description ?? string.Empty,
                IsPublic = audio.IsPublic,
                Duration = audio.Duration,
                FileSize = audio.FileSize,
                FileExt = audio.FileExt,
                Tags = audio.Tags.Select(tag => tag.TagId).ToArray(),
                FavoriteCount = audio.Favorited.Count,
                IsFavorited = currentUserId > 0
                              && audio.Favorited.Any(f => f.UserId == currentUserId),
                Created = audio.Created,
                Updated = audio.LastModified,
                User = new UserViewModel
                {
                    Id = audio.User.Id,
                    Username = audio.User.UserName,
                },
            };
        }

        public static Expression<Func<Audio, AudioListViewModel>> AudioList(long currentUserId)
        {
            return audio => new AudioListViewModel
            {
                Id = audio.Id,
                Title = audio.Title,
                IsPublic = audio.IsPublic,
                FavoriteCount = audio.Favorited.Count,
                IsFavorited = currentUserId > 0
                              && audio.Favorited.Any(f => f.UserId == currentUserId),
                Created = audio.Created,
                Updated = audio.LastModified,
                User = new UserViewModel
                {
                    Id = audio.User.Id,
                    Username = audio.User.UserName
                }
            };
        }

        public static Expression<Func<FollowedUser, FollowUserViewModel>> FollowUser(long currentUserId)
        {
            return u => new FollowUserViewModel
            {
                Id = u.Target.Id,
                Username = u.Target.UserName,
                AvatarUrl = string.Empty,
                IsFollowing = u.Target.Followers.Any(x => x.ObserverId == currentUserId)
            };
        }

        public static Expression<Func<User, UserDetailsViewModel>> UserDetails(long currentUserId)
        {
            return user => new UserDetailsViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                AboutMe = user.About ?? string.Empty,
                Website = user.Website ?? string.Empty,
                AudioCount = user.Audios.Count,
                FollowerCount = user.Followers.Count,
                FollowingCount = user.Followings.Count,
                IsFollowing = user.Followers.Any(f => f.ObserverId == currentUserId)
            };
        }
    }
}