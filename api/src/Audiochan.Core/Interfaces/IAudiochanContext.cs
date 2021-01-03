using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Interfaces
{
    public interface IAudiochanContext
    {
        DbSet<FavoriteAudio> FavoriteAudios { get; set; }
        DbSet<FollowedUser> FollowedUsers { get; set; }
        DbSet<Tag> Tags { get; set; }
        DbSet<Audio> Audios { get; set; }
        DbSet<AudioTag> AudioTags { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<Role> Roles { get; set; }
        DbSet<UserRole> UserRoles { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}