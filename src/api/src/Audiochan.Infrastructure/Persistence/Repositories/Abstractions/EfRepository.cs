using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Interfaces.Persistence;
using Audiochan.Domain.Abstractions;
using Audiochan.Infrastructure.Persistence.Repositories.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories.Abstractions
{
    public class EfRepository<TEntity> : IEntityRepository<TEntity> where TEntity : class
    {
        protected readonly ApplicationDbContext DbContext;
        protected readonly DbSet<TEntity> DbSet;

        public EfRepository(ApplicationDbContext dbContext)
        {
            DbContext = dbContext;
            DbSet = dbContext.Set<TEntity>();
        }

        public async Task<TEntity?> FindAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            var ids = new object[] { id };
            return await DbSet.FindAsync(ids, cancellationToken);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            return await DbSet.Where(predicate).CountAsync(ct);
        }

        public async Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).CountAsync(ct);
        }

        public async Task<int> CountAsync<TDto>(ISpecification<TEntity, TDto> specification, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).CountAsync(ct);
        }

        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            return await DbSet.Where(predicate).AnyAsync(ct);
        }

        public async Task<bool> ExistsAsync(ISpecification<TEntity> specification, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).AnyAsync(ct);
        }

        public async Task<bool> ExistsAsync<TDto>(ISpecification<TEntity, TDto> specification, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).AnyAsync(ct);
        }

        public async Task<TEntity?> GetFirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            return await DbSet.Where(predicate).FirstOrDefaultAsync(ct);
        }

        public async Task<TEntity?> GetFirstAsync(ISpecification<TEntity> specification, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).FirstOrDefaultAsync(ct);
        }

        public async Task<TDto?> GetFirstAsync<TDto>(ISpecification<TEntity, TDto> specification, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).FirstOrDefaultAsync(ct);
        }

        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            return await DbSet.Where(predicate).ToListAsync(ct);
        }

        public async Task<List<TEntity>> GetListAsync(ISpecification<TEntity> specification, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).ToListAsync(ct);
        }

        public async Task<List<TDto>> GetListAsync<TDto>(ISpecification<TEntity, TDto> specification, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).ToListAsync(ct);
        }

        public async Task<List<TEntity>> GetPagedListAsync(ISpecification<TEntity> specification, int page, int size, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).PaginateAsync(page, size, ct);
        }

        public async Task<List<TDto>> GetPagedListAsync<TDto>(ISpecification<TEntity, TDto> specification, int page, int size, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).PaginateAsync(page, size, ct);
        }

        public async Task<List<TEntity>> GetOffsetPagedListAsync(ISpecification<TEntity> specification, int offset, int size, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).OffsetPaginateAsync(offset, size, ct);
        }

        public async Task<List<TDto>> GetOffsetPagedListAsync<TDto>(ISpecification<TEntity, TDto> specification, int offset, int size, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).OffsetPaginateAsync(offset, size, ct);
        }

        public async Task<List<TEntityWithId>> GetCursorPagedListAsync<TEntityWithId, TKey>(ISpecification<TEntityWithId> specification, TKey cursor, int size, CancellationToken ct = default) 
            where TEntityWithId : class, IHasId<TKey>
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            var evaluator = new SpecificationEvaluator();
            var queryable = evaluator.GetQuery(DbContext.Set<TEntityWithId>(), specification);
            return await queryable.CursorPaginateAsync(cursor, size, ct);
        }

        public async Task<List<TDto>> GetCursorPagedListAsync<TDto, TKey>(ISpecification<TEntity, TDto> specification, TKey cursor, int size, CancellationToken ct = default) 
            where TDto : IHasId<TKey>
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            return await ApplySpecification(specification).CursorPaginateAsync(cursor, size, ct);
        }

        public void Add(TEntity entity) => DbSet.Add(entity);
        public void AddRange(IEnumerable<TEntity> entities) => DbSet.AddRange(entities);
        public void AddRange(params TEntity[] entities) => DbSet.AddRange(entities);

        public async Task AddAsync(TEntity entity, CancellationToken ct = default) =>
            await DbSet.AddAsync(entity, ct);
        public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default) =>
            await DbSet.AddRangeAsync(entities, ct);

        public void Update(TEntity entity) => DbSet.Update(entity);
        public void UpdateRange(IEnumerable<TEntity> entities) => DbSet.UpdateRange(entities);
        public void UpdateRange(params TEntity[] entities) => DbSet.UpdateRange(entities);
        

        public void Remove(TEntity entity) => DbSet.Remove(entity);
        public void RemoveRange(IEnumerable<TEntity> entities) => DbSet.RemoveRange(entities);
        public void RemoveRange(params TEntity[] entities) => DbSet.RemoveRange(entities);

        protected IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> specification)
        {
            var evaluator = new SpecificationEvaluator();
            return evaluator.GetQuery(DbSet.AsQueryable(), specification);
        }

        protected IQueryable<TDto> ApplySpecification<TDto>(ISpecification<TEntity, TDto> specification)
        {
            var evaluator = new SpecificationEvaluator();
            return evaluator.GetQuery(DbSet.AsQueryable(), specification);
        }
    }
}