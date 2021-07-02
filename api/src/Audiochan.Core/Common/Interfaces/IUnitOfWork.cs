using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Repositories;

namespace Audiochan.Core.Common.Interfaces
{
    public interface IUnitOfWork
    {
        IAudioRepository Audios { get; }
        IPlaylistRepository Playlists { get; }
        ITagRepository Tags { get; }
        IUserRepository Users { get; }
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}