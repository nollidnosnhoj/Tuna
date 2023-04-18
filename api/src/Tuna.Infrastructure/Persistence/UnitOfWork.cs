using System;
using System.Threading;
using System.Threading.Tasks;
using Tuna.Application.Entities;
using Tuna.Application.Persistence;
using Tuna.Application.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Tuna.Infrastructure.Persistence.Repositories;

namespace Tuna.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork, IAsyncDisposable
    {
        private readonly ApplicationDbContext _dbContext;
        private IDbContextTransaction? _currentTransaction;
        public IAudioRepository Audios { get; }
        public IEntityRepository<FavoriteAudio> FavoriteAudios { get; }
        public IEntityRepository<FollowedUser> FollowedUsers { get; }
        public IUserRepository Users { get; }
        
        public UnitOfWork(IDbContextFactory<ApplicationDbContext> dbContextFactory)
        {
            _dbContext = dbContextFactory.CreateDbContext();
            FavoriteAudios = new EfRepository<FavoriteAudio>(_dbContext);
            FollowedUsers = new EfRepository<FollowedUser>(_dbContext);
            Audios = new AudioRepository(_dbContext);
            Users = new UserRepository(_dbContext);
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

        public ValueTask DisposeAsync()
        {
            return _dbContext.DisposeAsync();
        }
    }
}