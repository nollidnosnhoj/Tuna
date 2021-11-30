using System;
using System.Collections.Generic;
using System.Linq;
using Audiochan.Domain.Abstractions;
using Audiochan.Domain.Enums;

namespace Audiochan.Domain.Entities
{
    public class User : IAudited, IHasId<long>
    {
        public User(string username, string email, string passwordHash, UserRole userRole = UserRole.Regular)
        {
            this.UserName = username;
            this.Email = email;
            this.PasswordHash = passwordHash;
            this.Role = userRole;
        }

        public long Id { get; set; }
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public UserRole Role { get; set; }
        public string? Picture { get; set; }
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
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