using System;
using System.Linq.Expressions;
using Audiochan.Core.Features.Users.GetFollowers;
using Audiochan.Core.Features.Users.GetFollowings;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Features.Users
{
    public static class FollowedUserMaps
    {
        public static Expression<Func<FollowedUser, FollowerViewModel>> UserToFollowerFunc = user =>
            new FollowerViewModel
            {
                ObserverUserName = user.Observer.UserName,
                ObserverPicture = user.Observer.Picture,
                FollowedDate = user.FollowedDate
            };
        
        public static Expression<Func<FollowedUser, FollowingViewModel>> UserToFollowingFunc = user =>
            new FollowingViewModel
            {
                TargetUserName = user.Target.UserName,
                TargetPicture = user.Target.Picture,
                FollowedDate = user.FollowedDate
            };
    }
}