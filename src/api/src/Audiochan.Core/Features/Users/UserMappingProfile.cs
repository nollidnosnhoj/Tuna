﻿using System;
using System.Linq.Expressions;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Auth.GetCurrentUser;
using Audiochan.Core.Features.Users.GetProfile;

namespace Audiochan.Core.Features.Users
{
    public static class UserMaps
    {
        public static Expression<Func<User, CurrentUserViewModel>> UserToCurrentUserFunc = user =>
            new CurrentUserViewModel
            {
                Id = user.Id,
                Email = user.Email,
                Username = user.UserName
            };

        public static Expression<Func<User, ProfileViewModel>> UserToProfileFunc = user =>
            new ProfileViewModel
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