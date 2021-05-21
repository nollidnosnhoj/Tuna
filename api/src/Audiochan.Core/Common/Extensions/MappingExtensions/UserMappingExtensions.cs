using System;
using System.Linq;
using System.Linq.Expressions;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Auth;
using Audiochan.Core.Features.Auth.GetCurrentUser;
using Audiochan.Core.Features.Users;

namespace Audiochan.Core.Common.Extensions.MappingExtensions
{
    public static class UserMappingExtensions
    {
        public static IQueryable<CurrentUserViewModel> ProjectToCurrentUser(this IQueryable<User> queryable) =>
            queryable.Select(CurrentUserProjection());

        public static IQueryable<UserViewModel> ProjectToUser(this IQueryable<User> queryable, string userId, MediaStorageSettings storageSettings) =>
            queryable.Select(UserProjection(userId, storageSettings));

        public static UserViewModel MapToProfile(this User user, string userId, MediaStorageSettings storageSettings) =>
            UserProjection(userId, storageSettings).Compile().Invoke(user);

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
                    ? user.Followers.Any(f => f.ObserverId == userId)
                    : null
            };
        }
    }
}