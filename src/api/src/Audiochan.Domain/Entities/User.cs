using System;
using System.Collections.Generic;
using System.Linq;
using Audiochan.Domain.Abstractions;
using Audiochan.Domain.Enums;

namespace Audiochan.Domain.Entities
{
    public class User : IAudited, IHasId<long>
    {
        public User()
        {
            Audios = new HashSet<Audio>();
            FavoriteAudios = new HashSet<FavoriteAudio>();
            FavoritePlaylists = new HashSet<FavoritePlaylist>();
            Followings = new HashSet<FollowedUser>();
            Followers = new HashSet<FollowedUser>();
            Playlists = new HashSet<Playlist>();
            RefreshTokens = new HashSet<RefreshToken>();
        }

        public User(string username, string email, string passwordHash, UserRole userRole = UserRole.Regular) : this()
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
        public ICollection<Audio> Audios { get; set; }
        public ICollection<FavoriteAudio> FavoriteAudios { get; set; }
        public ICollection<FavoritePlaylist> FavoritePlaylists { get; set; }
        public ICollection<FollowedUser> Followings { get; set; }
        public ICollection<FollowedUser> Followers { get; set; }
        public ICollection<Playlist> Playlists { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }

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