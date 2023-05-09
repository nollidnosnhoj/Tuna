using System.Threading;
using System.Threading.Tasks;
using Tuna.Application.Persistence.Repositories;
using Tuna.Domain.Entities;

namespace Tuna.Application.Persistence;

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