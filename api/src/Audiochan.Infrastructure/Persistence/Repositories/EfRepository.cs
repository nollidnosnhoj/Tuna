using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Repositories;
using Audiochan.Core.Services;
using Audiochan.Infrastructure.Persistence.Extensions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public abstract class EfRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        protected readonly IMapper Mapper;
        protected readonly ApplicationDbContext DbContext;
        protected readonly DbSet<TEntity> DbSet;
        protected readonly ICurrentUserService CurrentUserService;

        protected EfRepository(ApplicationDbContext dbContext, ICurrentUserService currentUserService, IMapper mapper)
        {
            DbContext = dbContext;
            CurrentUserService = currentUserService;
            Mapper = mapper;
            DbSet = dbContext.Set<TEntity>();
        }
        
        public async Task<TEntity?> LoadAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
            where TKey : notnull
        {
            return await DbSet.FindAsync(new object[] {id}, cancellationToken);
        }

        public async Task<TEntity?> LoadAsync(Expression<Func<TEntity, bool>> predicate,
            bool shouldSetTracking = true,
            CancellationToken cancellationToken = default)
        {
            return await DbSet.Where(predicate)
                .SetTracking(shouldSetTracking)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Where(predicate)
                .AnyAsync(cancellationToken);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .Where(predicate)
                .CountAsync(cancellationToken);
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            await DbSet.AddAsync(entity, cancellationToken);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
        {
            await DbSet.AddRangeAsync(entities, cancellationToken);
        }

        public void Update(TEntity entity)
        {
            DbSet.Update(entity);
        }

        public void UpdateRange(IEnumerable<TEntity> entities)
        {
            DbSet.UpdateRange(entities);
        }

        public void Remove(TEntity entity)
        {
            DbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            DbSet.RemoveRange(entities);
        }
    }
}