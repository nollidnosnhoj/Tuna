using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

// ReSharper disable MethodHasAsyncOverloadWithCancellation

namespace Audiochan.Infrastructure.Persistence.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private IDbContextTransaction? _currentTransaction;
        
        public IAudioRepository Audios { get; }
        public IFollowedUserRepository FollowedUsers { get; }
        public ITagRepository Tags { get; }
        public IUserRepository Users { get; }

        public UnitOfWork(ApplicationDbContext dbContext, 
            IAudioRepository audios, 
            IFollowedUserRepository followedUsers, 
            ITagRepository tags, 
            IUserRepository users)
        {
            _dbContext = dbContext;
            Audios = audios;
            FollowedUsers = followedUsers;
            Tags = tags;
            Users = users;
        }

        public bool HasActiveTransaction => _currentTransaction != null;

        public void Dispose() => _dbContext.Dispose();
        
        public Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _currentTransaction ??= _dbContext.Database.BeginTransaction();
            return Task.CompletedTask;
        }
        
        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _currentTransaction?.Commit();
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
        }

        public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _currentTransaction?.Rollback();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }
            }
            
            return Task.CompletedTask;
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
            await _dbContext.SaveChangesAsync(cancellationToken);
    }
}