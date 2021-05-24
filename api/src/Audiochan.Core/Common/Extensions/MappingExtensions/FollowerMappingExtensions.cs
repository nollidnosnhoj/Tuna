using System;
using System.Linq;
using System.Linq.Expressions;
using Audiochan.Core.Common.Settings;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Followers;

namespace Audiochan.Core.Common.Extensions.MappingExtensions
{
    public static class FollowerMappingExtensions
    {
        public static IQueryable<FollowerViewModel> ProjectToFollower(this IQueryable<FollowedUser> queryable, MediaStorageSettings options) =>
            queryable.Select(FollowerToListProjection(options));

        public static IQueryable<FollowingViewModel> ProjectToFollowing(this IQueryable<FollowedUser> queryable, MediaStorageSettings options) =>
            queryable.Select(FollowingToListProjection(options));

        private static Expression<Func<FollowedUser, FollowerViewModel>> FollowerToListProjection(MediaStorageSettings options)
        {
            return followedUser => new FollowerViewModel
            {
                FollowedDate = followedUser.FollowedDate,
                Username = followedUser.Observer.UserName,
                Picture = followedUser.Observer.Picture != null
                    ? $"https://{options.Image.Bucket}.s3.amazonaws.com/{options.Image.Container}/users/{followedUser.Observer.Picture}"
                    : null
            };
        }

        private static Expression<Func<FollowedUser, FollowingViewModel>> FollowingToListProjection(MediaStorageSettings options)
        {
            return followedUser => new FollowingViewModel
            {
                FollowedDate = followedUser.FollowedDate,
                Picture = followedUser.Target.Picture != null
                    ? $"https://{options.Image.Bucket}.s3.amazonaws.com/{options.Image.Container}/users/{followedUser.Target.Picture}"
                    : null,
                Username = followedUser.Target.UserName
            };
        }
    }
}