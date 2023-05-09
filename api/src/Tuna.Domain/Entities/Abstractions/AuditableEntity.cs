namespace Tuna.Domain.Entities.Abstractions;

public interface IAuditable
{
    DateTime CreatedAt { get; set; }
    DateTime? UpdatedAt { get; set; }
}

public abstract class AuditableEntity<TKey> : BaseEntity<TKey>, IAuditable
    where TKey : IComparable<TKey>, IEquatable<TKey>
{
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}