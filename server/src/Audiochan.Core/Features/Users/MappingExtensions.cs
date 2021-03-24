using System;
using System.Linq;
using System.Linq.Expressions;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Users.GetCurrentUser;
using Audiochan.Core.Features.Users.GetUser;

namespace Audiochan.Core.Features.Users
{
    public static class UserMappingExtensions
    {
        public static Expression<Func<User, CurrentUserViewModel>> CurrentUserProjection()
        {
            return user => new CurrentUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.UserName
            };
        }

        public static Expression<Func<User, UserViewModel>> UserProjection(string userId)
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