using System;
using System.Linq.Expressions;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Followers.GetFollowers;
using Audiochan.Core.Features.Followers.GetFollowings;

namespace Audiochan.Core.Common.Mappings
{
    public static class FollowedUserMappings
    {
        public static Expression<Func<FollowedUser, FollowerViewModel>> FollowerToListProjection()
        {
            return followedUser => new FollowerViewModel
            {
                FollowedDate = followedUser.FollowedDate,
                Username = followedUser.Observer.UserName,
                Picture = followedUser.Observer.Picture != null
                    ? string.Format(MediaLinkInvariants.UserPictureUrl, followedUser.Observer.Picture)
                    : null
            };
        }

        public static Expression<Func<FollowedUser, FollowingViewModel>> FollowingToListProjection()
        {
            return followedUser => new FollowingViewModel
            {
                FollowedDate = followedUser.FollowedDate,
                Picture = followedUser.Target.Picture != null
                    ? string.Format(MediaLinkInvariants.UserPictureUrl, followedUser.Target.Picture)
                    : null,
                Username = followedUser.Target.UserName
            };
        }
    }
}