using System;
using System.Threading;
using System.Threading.Tasks;

namespace Audiochan.Core.Common.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAudioRepository Audios { get; }
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