using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Entities
{
    public sealed class User : IdentityUser
    {
        public User()
        {
            Audios = new HashSet<Audio>();
            FavoriteAudios = new HashSet<FavoriteAudio>();
            FavoritePlaylists = new HashSet<FavoritePlaylist>();
            Followings = new HashSet<FollowedUser>();
            Followers = new HashSet<FollowedUser>();
            RefreshTokens = new HashSet<RefreshToken>();
        }

        public User(string username, string email, DateTime joined) : this()
        {
            this.UserName = username;
            this.Email = email;
            this.Joined = joined;
            this.DisplayName = username;
        }

        public string DisplayName { get; set; } = null!;
        public string? PictureBlobName { get; set; }
        public string? About { get; set; }
        public string? Website { get; set; }
        public DateTime Joined { get; set; }
        public ICollection<Audio> Audios { get; set; }
        public ICollection<FavoriteAudio> FavoriteAudios { get; set; }
        public ICollection<FavoritePlaylist> FavoritePlaylists { get; set; }
        public ICollection<FollowedUser> Followings { get; set; }
        public ICollection<FollowedUser> Followers { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }

        public void UpdateDisplayName(string? displayName)
        {
            if (!string.IsNullOrWhiteSpace(displayName))
            {
                if (string.Equals(this.UserName.Trim(), displayName.Trim(),
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    this.DisplayName = displayName;
                }
            }
        }
    }
}