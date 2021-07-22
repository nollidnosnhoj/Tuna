using System;
using System.Linq.Expressions;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Followers.GetFollowers;
using Audiochan.Core.Features.Followers.GetFollowings;

namespace Audiochan.Core.Features.Followers
{
    public static class FollowedUserMaps
    {
        public static Expression<Func<FollowedUser, FollowerViewModel>> UserToFollowerFunc = user =>
            new FollowerViewModel
            {
                ObserverUserName = user.Observer.UserName,
                ObserverPicture = user.Observer.PictureBlobName != null
                    ? string.Format(MediaLinkInvariants.UserPictureUrl, user.Observer.PictureBlobName)
                    : null,
                FollowedDate = user.FollowedDate
            };
        
        public static Expression<Func<FollowedUser, FollowingViewModel>> UserToFollowingFunc = user =>
            new FollowingViewModel
            {
                TargetUserName = user.Target.UserName,
                TargetPicture = user.Target.PictureBlobName != null
                    ? string.Format(MediaLinkInvariants.UserPictureUrl, user.Target.PictureBlobName)
                    : null,
                FollowedDate = user.FollowedDate
            };
    }
}