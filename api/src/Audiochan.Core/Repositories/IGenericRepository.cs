using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Models;

namespace Audiochan.Core.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;
        
        Task<T?> GetAsync(Expression<Func<T, bool>> criteriaExpression, bool ignoreGlobalFilter = false,
            CancellationToken cancellationToken = default);
        Task<T?> GetAsync(ISpecification<T> specification, bool ignoreGlobalFilter = false, 
            CancellationToken cancellationToken = default);
        Task<TDto?> GetAsync<TDto>(ISpecification<T, TDto> specification, bool ignoreGlobalFilter = false, 
            CancellationToken cancellationToken = default) where TDto : class;
        
        Task<List<T>> GetListAsync(Expression<Func<T, bool>> criteriaExpression, bool ignoreGlobalFilter = false,
            CancellationToken cancellationToken = default);
        Task<List<T>> GetListAsync(ISpecification<T> specification, bool ignoreGlobalFilter = false,
            CancellationToken cancellationToken = default);
        Task<List<TDto>> GetListAsync<TDto>(ISpecification<T, TDto> specification, bool ignoreGlobalFilter = false, 
            CancellationToken cancellationToken = default) where TDto : class;

        Task<PagedListDto<TDto>> GetPagedListBySpec<TDto>(ISpecification<T, TDto> specification, int page, int size, bool ignoreGlobalFilter = false, 
            CancellationToken cancellationToken = default) where TDto : class;
        
        Task<bool> ExistsAsync(Expression<Func<T, bool>> criteriaExpression, bool ignoreGlobalFilter = false, 
            CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(ISpecification<T> specification, bool ignoreGlobalFilter = false, 
            CancellationToken cancellationToken = default);
        
        Task<int> CountAsync(Expression<Func<T, bool>> criteriaExpression, bool ignoreGlobalFilter = false, 
            CancellationToken cancellationToken = default);
        Task<int> CountAsync(ISpecification<T> specification, bool ignoreGlobalFilter = false, 
            CancellationToken cancellationToken = default);
        
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task<List<T>> AddRangeAsync(List<T> entities, CancellationToken cancellationToken = default);
        void Update(T entity);
        void UpdateRange(List<T> entities);
        void Remove(T entity);
        void RemoveRange(List<T> entities);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}