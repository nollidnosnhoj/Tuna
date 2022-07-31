using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Audiochan.Core.Persistence.Repositories
{
    public interface IEntityRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> FindAsync<TKey>(TKey id, CancellationToken cancellationToken = default)
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