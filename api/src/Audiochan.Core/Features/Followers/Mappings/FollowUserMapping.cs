using System;
using System.Linq;
using System.Linq.Expressions;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Followers.Models;

namespace Audiochan.Core.Features.Followers.Mappings
{
    public static class FollowUserMapping
    {
        public static Expression<Func<FollowedUser, FollowUserViewModel>> Map(string currentUserId)
        {
            return u => new FollowUserViewModel
            {
                Id = u.Target.Id,
                Username = u.Target.UserName,
                PictureUrl = u.Target.PictureUrl,
                IsFollowing = u.Target.Followers.Any(x => x.ObserverId == currentUserId)
            };
        }
    }
}