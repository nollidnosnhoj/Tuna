using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Common.Mappings;
using Audiochan.Domain.Abstractions;

namespace Audiochan.Core.Interfaces.Persistence
{
    public interface IEntityRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> FindAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
            where TKey : IEquatable<TKey>, IComparable<TKey>;

        #region CountAsync

        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
        
        Task<int> CountAsync(ISpecification<TEntity> specification, CancellationToken ct = default);

            #endregion

        #region ExistsAsync

        Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
    
        Task<bool> ExistsAsync(ISpecification<TEntity> specification, CancellationToken ct = default);

        #endregion

        #region GetFirstAsync

        Task<TEntity?> GetFirstAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);

        Task<TDto?> GetFirstAsync<TDto>(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
            where TDto : IMapFrom<TEntity>;

        Task<TEntity?> GetFirstAsync(ISpecification<TEntity> specification, CancellationToken ct = default);
        
        Task<TDto?> GetFirstAsync<TDto>(ISpecification<TEntity> specification, CancellationToken ct = default)
            where TDto : IMapFrom<TEntity>;
        
        Task<TDto?> GetFirstAsync<TDto>(ISpecification<TEntity, TDto> specification, CancellationToken ct = default);

        #endregion

        #region GetListAsync

        Task<List<TEntity>> GetListAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default);
        
        Task<List<TDto>> GetListAsync<TDto>(Expression<Func<TEntity, bool>> predicate, CancellationToken ct = default)
            where TDto : IMapFrom<TEntity>;
        
        Task<List<TEntity>> GetListAsync(ISpecification<TEntity> specification, CancellationToken ct = default);
        
        Task<List<TDto>> GetListAsync<TDto>(ISpecification<TEntity> specification, CancellationToken ct = default)
            where TDto : IMapFrom<TEntity>;
        
        Task<List<TDto>> GetListAsync<TDto>(ISpecification<TEntity, TDto> specification, CancellationToken ct = default);

        #endregion

        #region GetPagedListAsync

        Task<List<TEntity>> GetPagedListAsync(ISpecification<TEntity> specification, 
            int page, 
            int size,
            CancellationToken ct = default);
        
        Task<List<TDto>> GetPagedListAsync<TDto>(ISpecification<TEntity> specification, 
            int page, 
            int size,
            CancellationToken ct = default) where TDto : IMapFrom<TEntity>;

        #endregion

        #region GetOffsetPagedListAsync

        Task<List<TEntity>> GetOffsetPagedListAsync(ISpecification<TEntity> specification, 
            int offset,
            int size,
            CancellationToken ct = default);
        
        Task<List<TDto>> GetOffsetPagedListAsync<TDto>(ISpecification<TEntity> specification,
            int offset,
            int size,
            CancellationToken ct = default) where TDto : IMapFrom<TEntity>;

        #endregion

        #region GetCursorPagedListAsync

        Task<List<TDto>> GetCursorPagedListAsync<TDto, TKey>(
            ISpecification<TEntity> specification, TKey cursor, int size, CancellationToken ct = default) 
            where TDto : IHasId<TKey>, IMapFrom<TEntity>
            where TKey : IEquatable<TKey>, IComparable<TKey>;

        #endregion

        #region Add

        void Add(TEntity entity);
        
        void AddRange(IEnumerable<TEntity> entities);
        
        void AddRange(params TEntity[] entities);
        
        Task AddAsync(TEntity entity, CancellationToken ct = default);
        
        Task AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken ct = default);

        #endregion

        #region Update

        void Update(TEntity entity);
        
        void UpdateRange(IEnumerable<TEntity> entities);
        
        void UpdateRange(params TEntity[] entities);

        #endregion

        #region Remove

        void Remove(TEntity entity);
        
        void RemoveRange(IEnumerable<TEntity> entities);
        
        void RemoveRange(params TEntity[] entities);

        #endregion
    }
}