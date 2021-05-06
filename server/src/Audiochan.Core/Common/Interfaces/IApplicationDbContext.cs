using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Audio> Audios { get; }
        DbSet<FollowedUser> FollowedUsers { get; }
        DbSet<Tag> Tags { get; }
        DbSet<User> Users { get; }
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}