using Tuna.Domain.Entities.Abstractions;

namespace Tuna.Domain.Exceptions;

public abstract class EntityNotFoundException<T, TKey> : Exception
    where T : BaseEntity<TKey>
    where TKey : IEquatable<TKey>, IComparable<TKey>
{
    protected EntityNotFoundException(TKey id) : base($"{typeof(T).Name} with id {id} was not found.")
    {
        Id = id;
    }

    public string EntityName => typeof(T).Name;
    public TKey Id { get; }
}