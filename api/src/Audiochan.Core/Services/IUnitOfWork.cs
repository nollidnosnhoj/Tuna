using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Repositories;

namespace Audiochan.Core.Services
{
    public interface IUnitOfWork : IDisposable
    {
        IAudioRepository Audios { get; }
        IFavoriteAudioRepository FavoriteAudios { get; }
        IFollowedUserRepository FollowedUsers { get; }
        ITagRepository Tags { get; }
        IUserRepository Users { get; }

        bool HasActiveTransaction { get; }
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}