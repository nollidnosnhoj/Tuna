using System;
using System.Collections.Generic;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Entities.Abstractions;
using Audiochan.Core.Entities.Enums;

namespace Audiochan.Core.Entities
{
    public class User : IAudited, IResourceEntity<long>
    {
        public User()
        {
            Audios = new HashSet<Audio>();
            FavoriteAudios = new HashSet<Audio>();
            FavoritePlaylists = new HashSet<Playlist>();
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
        public ICollection<Audio> FavoriteAudios { get; set; }
        public ICollection<Playlist> FavoritePlaylists { get; set; }
        public ICollection<FollowedUser> Followings { get; set; }
        public ICollection<FollowedUser> Followers { get; set; }
        public ICollection<Playlist> Playlists { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}