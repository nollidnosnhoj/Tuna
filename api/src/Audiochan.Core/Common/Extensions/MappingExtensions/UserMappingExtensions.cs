using System;
using System.Linq;
using System.Linq.Expressions;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Auth.GetCurrentUser;
using Audiochan.Core.Features.Followers;
using Audiochan.Core.Features.Users;
using FastExpressionCompiler;

namespace Audiochan.Core.Common.Extensions.MappingExtensions
{
    public static class UserMappingExtensions
    {
        public static IQueryable<CurrentUserViewModel> ProjectToCurrentUser(this IQueryable<User> queryable) =>
            queryable.Select(CurrentUserProjection());

        public static IQueryable<UserViewModel> ProjectToUser(this IQueryable<User> queryable, string userId, MediaStorageSettings storageSettings) =>
            queryable.Select(UserProjection(userId, storageSettings));
        
        public static IQueryable<FollowerViewModel> ProjectToFollower(this IQueryable<User> queryable, MediaStorageSettings options) =>
            queryable.Select(FollowerToListProjection(options));

        public static IQueryable<FollowingViewModel> ProjectToFollowing(this IQueryable<User> queryable, MediaStorageSettings options) =>
            queryable.Select(FollowingToListProjection(options));

        public static UserViewModel MapToProfile(this User user, string userId, MediaStorageSettings storageSettings, bool returnNullIfFail = false) =>
            UserProjection(userId, storageSettings).CompileFast(returnNullIfFail).Invoke(user);

        private static Expression<Func<User, CurrentUserViewModel>> CurrentUserProjection()
        {
            return user => new CurrentUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.UserName
            };
        }

        private static Expression<Func<User, UserViewModel>> UserProjection(string userId, MediaStorageSettings options)
        {
            return user => new UserViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                About = user.About ?? "",
                Picture = user.Picture != null
                    ? $"https://{options.Image.Bucket}.s3.amazonaws.com/{options.Image.Container}/users/{user.Picture}"
                    : null,
                Website = user.Website ?? "",
                AudioCount = user.Audios.Count,
                FollowerCount = user.Followers.Count,
                FollowingCount = user.Followings.Count,
                IsFollowing = userId.Length > 0
                    ? user.Followers.Any(f => f.Id == userId)
                    : null
            };
        }
        
        private static Expression<Func<User, FollowerViewModel>> FollowerToListProjection(MediaStorageSettings options)
        {
            return user => new FollowerViewModel
            {
                Username = user.UserName,
                Picture = user.Picture != null
                    ? $"https://{options.Image.Bucket}.s3.amazonaws.com/{options.Image.Container}/users/{user.Picture}"
                    : null
            };
        }

        private static Expression<Func<User, FollowingViewModel>> FollowingToListProjection(MediaStorageSettings options)
        {
            return user => new FollowingViewModel
            {
                Picture = user.Picture != null
                    ? $"https://{options.Image.Bucket}.s3.amazonaws.com/{options.Image.Container}/users/{user.Picture}"
                    : null,
                Username = user.UserName
            };
        }
    }
}