using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Persistence.Repositories;

namespace Audiochan.Core.Persistence
{
    public interface IUnitOfWork
    {
        IAudioRepository Audios { get; }
        IEntityRepository<FavoriteAudio> FavoriteAudios { get; }
        IEntityRepository<FollowedUser> FollowedUsers { get; }
        IUserRepository Users { get; }

        Task BeginTransactionAsync();
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync();
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}