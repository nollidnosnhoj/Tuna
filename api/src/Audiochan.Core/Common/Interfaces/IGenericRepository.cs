using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using Audiochan.Core.Common.Models.Responses;

namespace Audiochan.Core.Common.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T?> GetByIdAsync<TId>(TId id, CancellationToken cancellationToken = default) where TId : notnull;
        Task<T?> GetBySpecAsync(ISpecification<T> specification, bool ignoreGlobalFilter = false, CancellationToken cancellationToken = default);
        Task<List<T>> GetListBySpecAsync(ISpecification<T> specification, bool ignoreGlobalFilter = false, 
            CancellationToken cancellationToken = default);
        Task<TDto?> GetBySpecAsync<TDto>(ISpecification<T, TDto> specification, bool ignoreGlobalFilter = false, 
            CancellationToken cancellationToken = default) where TDto : class;
        Task<List<TDto>> GetListBySpecAsync<TDto>(ISpecification<T, TDto> specification, bool ignoreGlobalFilter = false, 
            CancellationToken cancellationToken = default) where TDto : class;
        Task<PagedList<TDto>> GetPagedListBySpec<TDto>(ISpecification<T, TDto> specification, int page, int size, bool ignoreGlobalFilter = false, 
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
        Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task UpdateRangeAsync(List<T> entities, CancellationToken cancellationToken = default);
        Task RemoveAsync(T entity, CancellationToken cancellationToken = default);
        Task RemoveRangeAsync(List<T> entities, CancellationToken cancellationToken = default);
        Task SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}