using System;
using System.Linq;
using System.Linq.Expressions;
using Audiochan.Core.Entities;
using Audiochan.Core.Models.ViewModels;

namespace Audiochan.Core.Extensions.MappingExtensions
{
    public static class FollowerMappingExtensions
    {
        public static IQueryable<FollowerViewModel> ProjectToFollower(this IQueryable<FollowedUser> queryable) =>
            queryable.Select(FollowerToListProjection());

        public static IQueryable<FollowingViewModel> ProjectToFollowing(this IQueryable<FollowedUser> queryable) =>
            queryable.Select(FollowingToListProjection());
        
        private static Expression<Func<FollowedUser, FollowerViewModel>> FollowerToListProjection()
        {
            return user => new FollowerViewModel
            {
                Username = user.Observer.UserName,
                Picture = user.Observer.Picture
            };
        }

        private static Expression<Func<FollowedUser, FollowingViewModel>> FollowingToListProjection()
        {
            return user => new FollowingViewModel
            {
                Picture = user.Target.Picture,
                Username = user.Target.UserName
            };
        }
    }
}