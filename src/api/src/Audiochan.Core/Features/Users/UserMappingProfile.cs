using System;
using System.Linq.Expressions;
using Audiochan.Core.Common;
using Audiochan.Core.Features.Auth.GetCurrentUser;
using Audiochan.Core.Features.Users.GetProfile;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Features.Users
{
    public static class UserMaps
    {
        public static Expression<Func<User, CurrentUserDto>> UserToCurrentUserFunc = user =>
            new CurrentUserDto
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.UserName
            };

        public static Expression<Func<User, ProfileDto>> UserToProfileFunc = user =>
            new ProfileDto
            {
                Id = user.Id,
                Username = user.UserName,
                Picture = user.Picture != null
                    ? string.Format(MediaLinkInvariants.UserPictureUrl, user.Picture)
                    : null,
                AudioCount = user.Audios.Count,
                FollowerCount = user.Followers.Count,
                FollowingCount = user.Followings.Count
            };
    }
}