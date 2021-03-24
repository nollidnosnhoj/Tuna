using System;
using System.Linq.Expressions;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Followers.GetFollowers;
using Audiochan.Core.Features.Followers.GetFollowings;

namespace Audiochan.Core.Features.Followers
{
    public static class FollowerMappingExtensions
    {
        public static Expression<Func<FollowedUser, FollowerViewModel>> FollowerToListProjection()
        {
            return user => new FollowerViewModel
            {
                Username = user.Observer.UserName,
                Picture = user.Observer.Picture
            };
        }

        public static Expression<Func<FollowedUser, FollowingViewModel>> FollowingToListProjection()
        {
            return user => new FollowingViewModel
            {
                Picture = user.Target.Picture,
                Username = user.Target.UserName
            };
        }
    }
}