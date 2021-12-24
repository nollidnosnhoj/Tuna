using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Application.Commons.Services;
using Audiochan.Application.Persistence;
using Audiochan.Application.Persistence.Repositories;
using Audiochan.Domain.Abstractions;
using Audiochan.Domain.Entities;
using Audiochan.Infrastructure.Persistence.Repositories;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Audiochan.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly IDateTimeProvider _dateTimeProvider;
        private IDbContextTransaction? _currentTransaction;
        public IAudioRepository Audios { get; }
        public IEntityRepository<FavoriteAudio> FavoriteAudios { get; }
        public IEntityRepository<FollowedUser> FollowedUsers { get; }
        public IUserRepository Users { get; }
        
        public UnitOfWork(IDateTimeProvider dateTimeProvider, ApplicationDbContext dbContext, IMapper mapper)
        {
            _dateTimeProvider = dateTimeProvider;
            _dbContext = dbContext;
            FavoriteAudios = new EfRepository<FavoriteAudio>(dbContext, mapper);
            FollowedUsers = new EfRepository<FollowedUser>(dbContext, mapper);
            Audios = new AudioRepository(dbContext, mapper);
            Users = new UserRepository(dbContext, mapper);
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
            foreach (var entry in _dbContext.ChangeTracker.Entries<IAudited>())
            {
                var now = _dateTimeProvider.Now;
                if (entry.State == EntityState.Added && entry.Entity.Created == default)
                {
                    entry.Property(nameof(IAudited.Created)).CurrentValue = now;
                }

                if (entry.State == EntityState.Modified && entry.Entity.LastModified == default)
                {
                    entry.Property(nameof(IAudited.LastModified)).CurrentValue = now;
                }
            }
            
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}