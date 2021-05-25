using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public class EfRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext DbContext;
        protected readonly DbSet<T> DbSet;
        private readonly ISpecificationEvaluator _specificationEvaluator;

        public EfRepository(ApplicationDbContext dbContext) : this(dbContext, SpecificationEvaluator.Default)
        {
            
        }

        public EfRepository(ApplicationDbContext dbContext, ISpecificationEvaluator specificationEvaluator)
        {
            this.DbContext = dbContext;
            this.DbSet = dbContext.Set<T>();
            this._specificationEvaluator = specificationEvaluator;
        }

        public virtual async Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) 
            where TId : notnull
        {
            return await DbSet.FindAsync(new object[] {id}, cancellationToken: cancellationToken);
        }

        public virtual async Task<T?> GetBySpecAsync(ISpecification<T> specification, bool ignoreGlobalFilter = false,
            CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification)
                .IgnoreGlobalQueryFilters(ignoreGlobalFilter)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<List<T>> GetListBySpecAsync(ISpecification<T> specification, bool ignoreGlobalFilter = false, 
            CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification)
                .IgnoreGlobalQueryFilters(ignoreGlobalFilter)
                .ToListAsync(cancellationToken);
        }

        public virtual async Task<TDto?> GetBySpecAsync<TDto>(ISpecification<T, TDto> specification, bool ignoreGlobalFilter = false, 
            CancellationToken cancellationToken = default) where TDto : class
        {
            return await ApplySpecification(specification)
                .IgnoreGlobalQueryFilters(ignoreGlobalFilter)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<List<TDto>> GetListBySpecAsync<TDto>(ISpecification<T, TDto> specification, bool ignoreGlobalFilter = false, 
            CancellationToken cancellationToken = default) where TDto : class
        {
            return await ApplySpecification(specification)
                .IgnoreGlobalQueryFilters(ignoreGlobalFilter)
                .ToListAsync(cancellationToken);
        }

        public virtual async Task<PagedList<TDto>> GetPagedListBySpec<TDto>(ISpecification<T, TDto> specification, int page, int size, 
            bool ignoreGlobalFilter = false, CancellationToken cancellationToken = default) where TDto : class
        {
            return await ApplySpecification(specification)
                .IgnoreGlobalQueryFilters(ignoreGlobalFilter)
                .PaginateAsync(page, size, cancellationToken);
        }

        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>> criteriaExpression, bool ignoreGlobalFilter = false,
            CancellationToken cancellationToken = default)
        {
            return await DbSet.Where(criteriaExpression)
                .IgnoreGlobalQueryFilters(ignoreGlobalFilter)
                .AnyAsync(cancellationToken);
        }

        public virtual async Task<bool> ExistsAsync(ISpecification<T> specification, bool ignoreGlobalFilter = false, 
            CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification)
                .IgnoreGlobalQueryFilters(ignoreGlobalFilter)
                .AnyAsync(cancellationToken);
        }

        public virtual async Task<int> CountAsync(Expression<Func<T, bool>> criteriaExpression, bool ignoreGlobalFilter = false,
            CancellationToken cancellationToken = default)
        {
            return await DbSet.Where(criteriaExpression)
                .IgnoreGlobalQueryFilters(ignoreGlobalFilter)
                .CountAsync(cancellationToken);
        }

        public virtual async Task<int> CountAsync(ISpecification<T> specification, bool ignoreGlobalFilter = false, 
            CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification)
                .IgnoreGlobalQueryFilters(ignoreGlobalFilter)
                .CountAsync(cancellationToken);
        }

        public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await DbSet.AddAsync(entity, cancellationToken);
            await DbContext.SaveChangesAsync(cancellationToken);
            return entity;
        }

        public virtual async Task<List<T>> AddRangeAsync(List<T> entities, CancellationToken cancellationToken = default)
        {
            await DbSet.AddRangeAsync(entities, cancellationToken);
            await DbContext.SaveChangesAsync(cancellationToken);
            return entities;
        }

        public virtual async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            DbSet.Update(entity);
            await DbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task UpdateRangeAsync(List<T> entities, CancellationToken cancellationToken = default)
        {
            DbSet.UpdateRange(entities);
            await DbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task RemoveAsync(T entity, CancellationToken cancellationToken = default)
        {
            DbSet.Remove(entity);
            await DbContext.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task RemoveRangeAsync(List<T> entities, CancellationToken cancellationToken = default)
        {
            DbSet.RemoveRange(entities);
            await DbContext.SaveChangesAsync(cancellationToken);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await DbContext.SaveChangesAsync(cancellationToken);
        }

        protected virtual IQueryable<T> ApplySpecification(ISpecification<T> specification)
        {
            return _specificationEvaluator.GetQuery(DbSet.AsQueryable(), specification);
        }

        protected virtual IQueryable<TDto> ApplySpecification<TDto>(ISpecification<T, TDto> specification)
        {
            return _specificationEvaluator.GetQuery(DbSet.AsQueryable(), specification);
        }
    }
}