using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Persistence;
using Audiochan.Core.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public class EfRepository<TEntity> : IEntityRepository<TEntity> where TEntity : class
    {
        protected readonly ApplicationDbContext DbContext;
        protected IQueryable<TEntity> Queryable;
        private readonly DbSet<TEntity> _dbSet;

        // ReSharper disable once MemberCanBeProtected.Global
        public EfRepository(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
            _dbSet = DbContext.Set<TEntity>();
            Queryable = _dbSet.AsTracking();
        }

        public async Task<TEntity?> FindAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            var ids = new object[] { id };
            return await _dbSet.FindAsync(ids, cancellationToken);
        }
        
        public async Task AddAsync(TEntity entity, CancellationToken ct = default) =>
            await _dbSet.AddAsync(entity, ct);

        public async Task<TEntity> AddAndSaveChangesAsync(TEntity entity, CancellationToken ct = default)
        {
            var entityState = await _dbSet.AddAsync(entity, ct);
            await DbContext.SaveChangesAsync(ct);
            return entityState.Entity;
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default) =>
            await _dbSet.AddRangeAsync(entities, ct);
        
        public void Update(TEntity entity) => _dbSet.Update(entity);
        
        public void UpdateRange(IEnumerable<TEntity> entities) => _dbSet.UpdateRange(entities);
        
        public void UpdateRange(params TEntity[] entities) => _dbSet.UpdateRange(entities);
        
        public void Remove(TEntity entity) => _dbSet.Remove(entity);
        
        public void RemoveRange(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);
        
        public void RemoveRange(params TEntity[] entities) => _dbSet.RemoveRange(entities);
    }
}