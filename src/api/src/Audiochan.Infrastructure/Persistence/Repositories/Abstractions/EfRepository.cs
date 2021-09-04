using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Ardalis.Specification.EntityFrameworkCore;
using Audiochan.Core.Common.Interfaces.Persistence;
using Audiochan.Core.Common.Mappings;
using Audiochan.Domain.Abstractions;
using Audiochan.Infrastructure.Persistence.Repositories.Extensions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories.Abstractions
{
    public class EfRepository<TEntity> : IEntityRepository<TEntity> where TEntity : class
    {
        protected readonly ApplicationDbContext DbContext;
        protected readonly DbSet<TEntity> DbSet;
        protected readonly IMapper Mapper;

        // ReSharper disable once MemberCanBeProtected.Global
        public EfRepository(ApplicationDbContext dbContext, IMapper mapper)
        {
            DbContext = dbContext;
            DbSet = dbContext.Set<TEntity>();
            Mapper = mapper;
        }

        public async Task<TEntity?> FindAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            var ids = new object[] { id };
            return await DbSet.FindAsync(ids, cancellationToken);
        }
        
        #region CountAsync

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            return await DbSet.Where(predicate).CountAsync(ct);
        }

        public async Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).CountAsync(ct);
        }
        
        #endregion

        #region ExistsAsync
        
        public async Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            return await DbSet.Where(predicate).AnyAsync(ct);
        }

        public async Task<bool> ExistsAsync(ISpecification<TEntity> specification, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).AnyAsync(ct);
        }
        
        #endregion

        #region GetFirstAsync
        
        public async Task<TEntity?> GetFirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            return await DbSet.Where(predicate).FirstOrDefaultAsync(ct);
        }

        public async Task<TDto?> GetFirstAsync<TDto>(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default) 
            where TDto : IMapFrom<TEntity>
        {
            return await DbSet.Where(predicate)
                .ProjectTo<TDto>(Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<TEntity?> GetFirstAsync(ISpecification<TEntity> specification, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).FirstOrDefaultAsync(ct);
        }

        public async Task<TDto?> GetFirstAsync<TDto>(ISpecification<TEntity> specification, CancellationToken ct = default)
            where TDto : IMapFrom<TEntity>
        {
            return await ApplySpecification(specification)
                .ProjectTo<TDto>(Mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<TDto?> GetFirstAsync<TDto>(ISpecification<TEntity, TDto> specification, CancellationToken ct = default)
        {
            return await ApplySpecification(specification)
                .FirstOrDefaultAsync(ct);
        }

        #endregion

        #region GetListAsync
        
        public async Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
        {
            return await DbSet.Where(predicate).ToListAsync(ct);
        }

        public async Task<List<TDto>> GetListAsync<TDto>(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default) where TDto : IMapFrom<TEntity>
        {
            return await DbSet.Where(predicate)
                .ProjectTo<TDto>(Mapper.ConfigurationProvider)
                .ToListAsync(ct);
        }

        public async Task<List<TEntity>> GetListAsync(ISpecification<TEntity> specification, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).ToListAsync(ct);
        }

        public async Task<List<TDto>> GetListAsync<TDto>(ISpecification<TEntity> specification, CancellationToken ct = default)
            where TDto : IMapFrom<TEntity>
        {
            return await ApplySpecification(specification)
                .ProjectTo<TDto>(Mapper.ConfigurationProvider)
                .ToListAsync(ct);
        }

        public async Task<List<TDto>> GetListAsync<TDto>(ISpecification<TEntity, TDto> specification, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).ToListAsync(ct);
        }

        #endregion

        #region GetPagedListAsync
        
        public async Task<List<TEntity>> GetPagedListAsync(ISpecification<TEntity> specification, int page, int size, 
            CancellationToken ct = default)
        {
            return await ApplySpecification(specification).PaginateAsync(page, size, ct);
        }

        public async Task<List<TDto>> GetPagedListAsync<TDto>(ISpecification<TEntity> specification, int page, int size,
            CancellationToken ct = default) where TDto : IMapFrom<TEntity>
        {
            return await ApplySpecification(specification)
                .ProjectTo<TDto>(Mapper.ConfigurationProvider)
                .PaginateAsync(page, size, ct);
        }
        
        #endregion

        #region GetOffsetPagedListAsync
        
        public async Task<List<TEntity>> GetOffsetPagedListAsync(ISpecification<TEntity> specification, int offset, 
            int size, CancellationToken ct = default)
        {
            return await ApplySpecification(specification).OffsetPaginateAsync(offset, size, ct);
        }

        public async Task<List<TDto>> GetOffsetPagedListAsync<TDto>(ISpecification<TEntity> specification, int offset, 
            int size, CancellationToken ct = default) where TDto : IMapFrom<TEntity>
        {
            return await ApplySpecification(specification)
                .ProjectTo<TDto>(Mapper.ConfigurationProvider)
                .OffsetPaginateAsync(offset, size, ct);
        }
        
        #endregion

        #region GetCursorPagedListAsync
        
        public async Task<List<TDto>> GetCursorPagedListAsync<TDto, TKey>(ISpecification<TEntity> specification, 
            TKey cursor, int size, CancellationToken ct = default) 
            where TDto : IHasId<TKey>, IMapFrom<TEntity>
            where TKey : IEquatable<TKey>, IComparable<TKey>
        {
            return await ApplySpecification(specification)
                .ProjectTo<TDto>(Mapper.ConfigurationProvider)
                .CursorPaginateAsync(cursor, size, ct);
        }
        
        #endregion

        #region Add
        
        public void Add(TEntity entity) => DbSet.Add(entity);
        
        public void AddRange(IEnumerable<TEntity> entities) => DbSet.AddRange(entities);
        
        public void AddRange(params TEntity[] entities) => DbSet.AddRange(entities);
        
        public async Task AddAsync(TEntity entity, CancellationToken ct = default) =>
            await DbSet.AddAsync(entity, ct);
        
        public async Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default) =>
            await DbSet.AddRangeAsync(entities, ct);
        
        #endregion

        #region Update

        public void Update(TEntity entity) => DbSet.Update(entity);
        
        public void UpdateRange(IEnumerable<TEntity> entities) => DbSet.UpdateRange(entities);
        
        public void UpdateRange(params TEntity[] entities) => DbSet.UpdateRange(entities);

        #endregion
        
        #region Remove
        
        public void Remove(TEntity entity) => DbSet.Remove(entity);
        
        public void RemoveRange(IEnumerable<TEntity> entities) => DbSet.RemoveRange(entities);
        
        public void RemoveRange(params TEntity[] entities) => DbSet.RemoveRange(entities);
        
        #endregion

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