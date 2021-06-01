using System;
using System.Linq.Expressions;
using Audiochan.API.Features.Followers.GetFollowers;
using Audiochan.API.Features.Followers.GetFollowings;
using Audiochan.Core.Constants;
using Audiochan.Core.Entities;

namespace Audiochan.API.Mappings
{
    public static class FollowedUserMappings
    {
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