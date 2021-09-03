using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Domain.Abstractions;

namespace Audiochan.Core.Interfaces.Persistence
{
    public interface IEntityRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> FindAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
            where TKey : IEquatable<TKey>, IComparable<TKey>;

        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
        Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken ct = default);
        Task<int> CountAsync<TDto>(ISpecification<TEntity, TDto> specification, CancellationToken ct = default);
        
        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
        Task<bool> ExistsAsync(ISpecification<TEntity> specification, CancellationToken ct = default);
        Task<bool> ExistsAsync<TDto>(ISpecification<TEntity, TDto> specification, CancellationToken ct = default);
        
        Task<TEntity?> GetFirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
        Task<TEntity?> GetFirstAsync(ISpecification<TEntity> specification, CancellationToken ct = default);
        Task<TDto?> GetFirstAsync<TDto>(ISpecification<TEntity, TDto> specification, CancellationToken ct = default);
        
        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
        Task<List<TEntity>> GetListAsync(ISpecification<TEntity> specification, CancellationToken ct = default);
        Task<List<TDto>> GetListAsync<TDto>(ISpecification<TEntity, TDto> specification, CancellationToken ct = default);
        
        Task<List<TEntity>> GetPagedListAsync(ISpecification<TEntity> specification, 
            int page, 
            int size,
            CancellationToken ct = default);
        Task<List<TDto>> GetPagedListAsync<TDto>(ISpecification<TEntity, TDto> specification, 
            int page, 
            int size,
            CancellationToken ct = default);
        
        Task<List<TEntity>> GetOffsetPagedListAsync(ISpecification<TEntity> specification, 
            int offset,
            int size,
            CancellationToken ct = default);
        Task<List<TDto>> GetOffsetPagedListAsync<TDto>(ISpecification<TEntity, TDto> specification,
            int offset,
            int size,
            CancellationToken ct = default);

        Task<List<TEntityWithId>> GetCursorPagedListAsync<TEntityWithId, TKey>(
            ISpecification<TEntityWithId> specification, TKey cursor, int size, CancellationToken ct = default)
            where TEntityWithId : class, IHasId<TKey>
            where TKey : IEquatable<TKey>, IComparable<TKey>;
        
        Task<List<TDto>> GetCursorPagedListAsync<TDto, TKey>(
            ISpecification<TEntity, TDto> specification, TKey cursor, int size, CancellationToken ct = default) 
            where TDto : IHasId<TKey>
            where TKey : IEquatable<TKey>, IComparable<TKey>;

        void Add(TEntity entity);
        void AddRange(IEnumerable<TEntity> entities);
        void AddRange(params TEntity[] entities);
        
        Task AddAsync(TEntity entity, CancellationToken ct = default);
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

        void Update(TEntity entity);
        void UpdateRange(IEnumerable<TEntity> entities);
        void UpdateRange(params TEntity[] entities);
        
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        void RemoveRange(params TEntity[] entities);
    }
}