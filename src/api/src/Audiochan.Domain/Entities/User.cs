using System;
using System.Collections.Generic;
using System.Linq;
using Audiochan.Domain.Abstractions;
using Audiochan.Domain.Enums;

namespace Audiochan.Domain.Entities
{
    public class User : IAudited, IHasId<long>
    {
        public User(string userName, string email, string passwordHash)
        {
            this.UserName = userName;
            this.Email = email;
            this.PasswordHash = passwordHash;
        }

        public long Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string UserType { get; set; } = null!;
        public DateTime Created { get; set; }
        public DateTime? LastModified { get; set; }
        public ICollection<FavoriteAudio> FavoriteAudios { get; set; } = new HashSet<FavoriteAudio>();
        public ICollection<FollowedArtist> Followings { get; set; } = new HashSet<FollowedArtist>();
    }
}