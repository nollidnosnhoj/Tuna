using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Audiochan.Core.Extensions;
using Audiochan.Core.Models;
using Audiochan.Core.Persistence;
using Audiochan.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    internal class EfRepository<T> : IGenericRepository<T> where T : class
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

        public async Task<T?> GetAsync(Expression<Func<T, bool>> criteriaExpression, bool ignoreGlobalFilter = false,
            CancellationToken cancellationToken = default)
        {
            return await DbSet.Where(criteriaExpression)
                .IgnoreGlobalQueryFilters(ignoreGlobalFilter)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<T?> GetAsync(ISpecification<T> specification, bool ignoreGlobalFilter = false,
            CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification)
                .IgnoreGlobalQueryFilters(ignoreGlobalFilter)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> criteriaExpression, bool ignoreGlobalFilter = false,
            CancellationToken cancellationToken = default)
        {
            return await DbSet.Where(criteriaExpression)
                .IgnoreGlobalQueryFilters(ignoreGlobalFilter)
                .ToListAsync(cancellationToken);
        }

        public virtual async Task<List<T>> GetListAsync(ISpecification<T> specification, bool ignoreGlobalFilter = false, 
            CancellationToken cancellationToken = default)
        {
            return await ApplySpecification(specification)
                .IgnoreGlobalQueryFilters(ignoreGlobalFilter)
                .ToListAsync(cancellationToken);
        }

        public virtual async Task<TDto?> GetAsync<TDto>(ISpecification<T, TDto> specification, bool ignoreGlobalFilter = false, 
            CancellationToken cancellationToken = default) where TDto : class
        {
            return await ApplySpecification(specification)
                .IgnoreGlobalQueryFilters(ignoreGlobalFilter)
                .SingleOrDefaultAsync(cancellationToken);
        }

        public virtual async Task<List<TDto>> GetListAsync<TDto>(ISpecification<T, TDto> specification, bool ignoreGlobalFilter = false, 
            CancellationToken cancellationToken = default) where TDto : class
        {
            return await ApplySpecification(specification)
                .IgnoreGlobalQueryFilters(ignoreGlobalFilter)
                .ToListAsync(cancellationToken);
        }

        public virtual async Task<PagedListDto<TDto>> GetPagedListBySpec<TDto>(ISpecification<T, TDto> specification, int page, int size, 
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
            return entity;
        }

        public virtual async Task<List<T>> AddRangeAsync(List<T> entities, CancellationToken cancellationToken = default)
        {
            await DbSet.AddRangeAsync(entities, cancellationToken);
            return entities;
        }

        public virtual void Update(T entity)
        {
            DbSet.Update(entity);
        }

        public virtual void UpdateRange(List<T> entities)
        {
            DbSet.UpdateRange(entities);
        }

        public virtual void Remove(T entity)
        {
            DbSet.Remove(entity);
        }

        public virtual void RemoveRange(List<T> entities)
        {
            DbSet.RemoveRange(entities);
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