using System.Threading;
using System.Threading.Tasks;
using Audiochan.Domain.Entities;

namespace Audiochan.Core.Common.Interfaces.Persistence
{
    public interface IUnitOfWork
    {
        IAudioRepository Audios { get; }
        IEntityRepository<FavoriteAudio> FavoriteAudios { get; }
        IEntityRepository<FavoritePlaylist> FavoritePlaylists { get; }
        IEntityRepository<FollowedUser> FollowedUsers { get; }
        IEntityRepository<PlaylistAudio> PlaylistAudios { get; }
        IPlaylistRepository Playlists { get; }
        ITagRepository Tags { get; }
        IUserRepository Users { get; }

        Task BeginTransactionAsync();
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync();
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}