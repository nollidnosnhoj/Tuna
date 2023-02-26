using System;
using System.Collections.Generic;
using System.Linq;
using Audiochan.Common.Abstractions;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Entities
{
    public sealed class User : IdentityUser<long>, IAuditable
    {
        private User()
        {
            
        }
        
        public User(string userName, string email)
        {
            this.UserName = userName;
            this.Email = email;
        }
        
        public string? ImageId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // navigational properties
        public ICollection<Audio> Audios { get; set; } = new HashSet<Audio>();
        public ICollection<FavoriteAudio> FavoriteAudios { get; set; } = new HashSet<FavoriteAudio>();
        public ICollection<FollowedUser> Followings { get; set; } = new HashSet<FollowedUser>();
        public ICollection<FollowedUser> Followers { get; set; } = new HashSet<FollowedUser>();

        public void Follow(long observerId, DateTime followedDate)
        {
            var follower = this.Followers.FirstOrDefault(f => f.ObserverId == observerId);

            if (follower is null)
            {
                follower = new FollowedUser
                {
                    TargetId = this.Id,
                    ObserverId = observerId,
                    FollowedDate = followedDate
                };
                
                this.Followers.Add(follower);
            }
            else if (follower.UnfollowedDate is not null)
            {
                follower.FollowedDate = followedDate;
                follower.UnfollowedDate = null;
            }
        }

        public void UnFollow(long observerId, DateTime unfollowedDate)
        {
            var follower = this.Followers.FirstOrDefault(f => f.ObserverId == observerId);

            if (follower is not null)
            {
                follower.UnfollowedDate = unfollowedDate;
            }
        }
    }
}