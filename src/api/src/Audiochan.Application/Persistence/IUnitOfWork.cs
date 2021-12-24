using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Persistence.Repositories;
using Audiochan.Domain.Entities;

namespace Audiochan.Application.Persistence
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