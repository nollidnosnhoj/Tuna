using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Repositories;

namespace Audiochan.Core.Common.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IAudioRepository Audios { get; }
        ITagRepository Tags { get; }
        IUserRepository Users { get; }
        void BeginTransaction();
        Task CommitTransactionAsync();
        void RollbackTransaction();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}