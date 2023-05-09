namespace Tuna.Domain.Entities.Abstractions;

public abstract class BaseEntity<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
{
    public TKey Id { get; set; } = default!;
}