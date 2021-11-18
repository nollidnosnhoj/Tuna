using System.Threading;
using System.Threading.Tasks;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Common.Interfaces.Persistence
{
    public interface IUnitOfWork
    {
        IArtistRepository Artists { get; }
        IAudioRepository Audios { get; }
        IEntityRepository<FavoriteAudio> FavoriteAudios { get; }
        IEntityRepository<FollowedArtist> FollowedUsers { get; }
        IUserRepository Users { get; }

        Task BeginTransactionAsync();
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync();
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}