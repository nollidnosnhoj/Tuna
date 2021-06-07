using System;
using System.Linq;
using System.Linq.Expressions;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Followers.GetFollowers;
using Audiochan.Core.Features.Followers.GetFollowings;

namespace Audiochan.Core.Common.Mappings
{
    public static class FollowedUserMappings
    {
        public static IQueryable<FollowerViewModel> ProjectToFollower(this IQueryable<FollowedUser> queryable) =>
            queryable.Select(FollowerToListProjection());

        public static IQueryable<FollowingViewModel> ProjectToFollowing(this IQueryable<FollowedUser> queryable) =>
            queryable.Select(FollowingToListProjection());
        
        public static Expression<Func<FollowedUser, FollowerViewModel>> FollowerToListProjection()
        {
            return followedUser => new FollowerViewModel
            {
                FollowedDate = followedUser.FollowedDate,
                Username = followedUser.Observer.UserName,
                Picture = followedUser.Observer.PictureBlobName != null
                    ? string.Format(MediaLinkInvariants.UserPictureUrl, followedUser.Observer.PictureBlobName)
                    : null
            };
        }

        public static Expression<Func<FollowedUser, FollowingViewModel>> FollowingToListProjection()
        {
            return followedUser => new FollowingViewModel
            {
                FollowedDate = followedUser.FollowedDate,
                Picture = followedUser.Target.PictureBlobName != null
                    ? string.Format(MediaLinkInvariants.UserPictureUrl, followedUser.Target.PictureBlobName)
                    : null,
                Username = followedUser.Target.UserName
            };
        }
    }
}