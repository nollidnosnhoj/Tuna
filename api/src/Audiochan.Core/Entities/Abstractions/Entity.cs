using System;

namespace Audiochan.Core.Entities.Abstractions;

public abstract class Entity<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
{
    public TKey Id { get; set; }
}