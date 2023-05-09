using Tuna.Domain.Entities.Abstractions;

namespace Tuna.Domain.Exceptions;

public class EntityNotFoundException<T, TKey> : Exception
    where T : BaseEntity<TKey>
    where TKey : IEquatable<TKey>, IComparable<TKey>
{
    public EntityNotFoundException(TKey id) : base($"{typeof(T).Name} with id {id} was not found.")
    {
        Id = id;
    }

    public TKey Id { get; }
}