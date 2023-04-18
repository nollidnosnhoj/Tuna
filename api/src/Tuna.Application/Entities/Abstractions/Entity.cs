using System;

namespace Tuna.Application.Entities.Abstractions;

public abstract class Entity<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
{
    public TKey Id { get; set; }
}