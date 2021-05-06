using System;
using System.Linq;
using System.Linq.Expressions;
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

        public static IQueryable<UserViewModel> ProjectToUser(this IQueryable<User> queryable, string userId) =>
            queryable.Select(UserProjection(userId));

        private static Expression<Func<User, CurrentUserViewModel>> CurrentUserProjection()
        {
            return user => new CurrentUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.UserName
            };
        }

        private static Expression<Func<User, UserViewModel>> UserProjection(string userId)
        {
            return user => new UserViewModel
            {
                Id = user.Id,
                Username = user.UserName,
                About = user.About,
                Picture = user.Picture,
                Website = user.Website,
                AudioCount = user.Audios.Count,
                FollowerCount = user.Followers.Count,
                FollowingCount = user.Followings.Count,
                IsFollowing = userId != null && userId.Length > 0
                    ? user.Followers.Any(f => f.ObserverId == userId)
                    : (bool?) null
            };
        }
    }
}