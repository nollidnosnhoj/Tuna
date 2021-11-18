using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Domain.Entities;
using Audiochan.Infrastructure.Persistence.Repositories;
using Audiochan.Infrastructure.Persistence.Repositories.Abstractions;
using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;

namespace Audiochan.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        private IDbContextTransaction? _currentTransaction;
        public IArtistRepository Artists { get; }
        public IAudioRepository Audios { get; }
        public IEntityRepository<FavoriteAudio> FavoriteAudios { get; }
        public IEntityRepository<FollowedArtist> FollowedArtists { get; }
        public IUserRepository Users { get; }
        
        public UnitOfWork(ApplicationDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            FavoriteAudios = new EfRepository<FavoriteAudio>(_dbContext, mapper);
            FollowedArtists = new EfRepository<FollowedArtist>(_dbContext, mapper);
            Artists = new ArtistRepository(_dbContext, mapper);
            Audios = new AudioRepository(_dbContext, mapper);
            Users = new UserRepository(_dbContext, mapper);
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