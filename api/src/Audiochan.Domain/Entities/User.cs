﻿using System;
using System.Collections.Generic;
using System.Linq;
using Audiochan.Domain.Abstractions;

namespace Audiochan.Domain.Entities
{
    public class User : AuditableEntity<long>
    {
        private User()
        {
            
        }
        
        public User(string identityId, string userName)
        {
            this.IdentityId = identityId;
            this.UserName = userName;
        }

        public string IdentityId { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string? ImageId { get; set; }
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