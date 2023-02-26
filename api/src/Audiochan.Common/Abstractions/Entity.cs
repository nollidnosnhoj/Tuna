namespace Audiochan.Common.Abstractions;

public interface IEntity<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
{
    public TKey Id { get; set; }
}