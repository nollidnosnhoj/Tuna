using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Entities
{
    public class User : IdentityUser
    {
        public User()
        {
            Audios = new HashSet<Audio>();
            FavoriteAudios = new HashSet<FavoriteAudio>();
            Followings = new HashSet<FollowedUser>();
            Followers = new HashSet<FollowedUser>();
            RefreshTokens = new HashSet<RefreshToken>();
        }

        public string DisplayName { get; set; }
        public string PictureUrl { get; set; }
        public string About { get; set; }
        public string Website { get; set; }
        public DateTime Joined { get; set; }
        public ICollection<Audio> Audios { get; set; }
        public ICollection<FavoriteAudio> FavoriteAudios { get; set; }
        public ICollection<FollowedUser> Followings { get; set; }
        public ICollection<FollowedUser> Followers { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
