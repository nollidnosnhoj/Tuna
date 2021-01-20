using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Audiochan.Core.Interfaces
{
    public interface IDbContext
    {
        DbSet<Audio> Audios { get; }
        // DbSet<AudioTag> AudioTags { get; }
        DbSet<FavoriteAudio> FavoriteAudios { get; }
        DbSet<FollowedUser> FollowedUsers { get; }
        DbSet<Genre> Genres { get; }
        DbSet<Tag> Tags { get; }
        DbSet<User> Users { get; }
        
        DatabaseFacade Database { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}