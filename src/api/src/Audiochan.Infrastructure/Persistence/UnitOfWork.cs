using System;
using System.Threading;
using System.Threading.Tasks;

using Audiochan.Application.Persistence;
using Audiochan.Application.Persistence.Repositories;
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
        public IEntityRepository<FollowedUser> FollowedUsers { get; }
        public IUserRepository Users { get; }
        
        public UnitOfWork(IAudioRepository audios,
            IUserRepository users, 
            IEntityRepository<FavoriteAudio> favoriteAudios, 
            IEntityRepository<FollowedUser> followedUsers, 
            ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
            FavoriteAudios = favoriteAudios;
            FollowedUsers = followedUsers;
            Audios = audios;
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