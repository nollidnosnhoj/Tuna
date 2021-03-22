using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Entities
{
    public sealed class User : IdentityUser
    {
        public User()
        {
            Audios = new HashSet<Audio>();
            Followings = new HashSet<FollowedUser>();
            Followers = new HashSet<FollowedUser>();
            RefreshTokens = new HashSet<RefreshToken>();
        }

        public User(string username, string email, DateTime joined)
        {
            this.UserName = username;
            this.Email = email;
            this.Joined = joined;
            this.DisplayName = username;
        }

        public string DisplayName { get; set; }
        public string Picture { get; set; }
        public string About { get; set; }
        public string Website { get; set; }
        public DateTime Joined { get; set; }
        public ICollection<Audio> Audios { get; set; }
        public ICollection<FollowedUser> Followings { get; set; }
        public ICollection<FollowedUser> Followers { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }

        public void UpdateDisplayName(string displayName)
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

        public void UpdateAbout(string about)
        {
            if (about is not null)
                this.About = about;
        }

        public void UpdateWebsite(string website)
        {
            if (website is not null)
                this.Website = website;
        }

        public void UpdatePicture(string picturePath)
        {
            if (!string.IsNullOrWhiteSpace(picturePath))
                this.Picture = picturePath;
        }

        public bool AddFollower(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            var follower = GetFollower(userId);

            if (follower is null)
            {
                follower = new FollowedUser {TargetId = this.Id, ObserverId = userId};
                this.Followers.Add(follower);
            }

            return true;
        }

        public bool RemoveFollower(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            var favorite = GetFollower(userId);

            if (favorite is not null)
                this.Followers.Remove(favorite);

            return false;
        }

        private FollowedUser GetFollower(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            return this.Followers.FirstOrDefault(f => f.ObserverId == userId);
        }
    }
}