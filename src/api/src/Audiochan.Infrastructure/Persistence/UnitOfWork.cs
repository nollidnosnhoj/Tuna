using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace Audiochan.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private IDbContextTransaction? _currentTransaction;
        public IAudioRepository Audios { get; }
        public IEntityRepository<FavoriteAudio> FavoriteAudios { get; }
        public IEntityRepository<FavoritePlaylist> FavoritePlaylists { get; }
        public IEntityRepository<FollowedUser> FollowedUsers { get; }
        public IEntityRepository<PlaylistAudio> PlaylistAudios { get; }
        public IPlaylistRepository Playlists { get; }
        public ITagRepository Tags { get; }
        public IUserRepository Users { get; }
        
        public UnitOfWork(IAudioRepository audios, 
            IPlaylistRepository playlists, 
            ITagRepository tags, 
            IUserRepository users, 
            IEntityRepository<FavoriteAudio> favoriteAudios, 
            IEntityRepository<FavoritePlaylist> favoritePlaylists, 
            IEntityRepository<FollowedUser> followedUsers, 
            IEntityRepository<PlaylistAudio> playlistAudios, 
            ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            FavoriteAudios = favoriteAudios;
            FavoritePlaylists = favoritePlaylists;
            FollowedUsers = followedUsers;
            PlaylistAudios = playlistAudios;
            Audios = audios;
            Playlists = playlists;
            Tags = tags;
            Users = users;
        }
        
        public Task BeginTransactionAsync()
        {
            if (_currentTransaction is not null)
                throw new InvalidOperationException("A transaction is already in progress.");

            _currentTransaction = _dbContext.Database.BeginTransaction();
            return Task.CompletedTask;
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
                // ReSharper disable once MethodHasAsyncOverloadWithCancellation
                _currentTransaction?.Commit();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_currentTransaction is not null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public Task RollbackTransactionAsync()
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction is not null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
            
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}