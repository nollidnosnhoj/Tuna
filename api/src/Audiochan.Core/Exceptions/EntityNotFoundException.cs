using System;
using Audiochan.Core.Entities.Abstractions;

namespace Audiochan.Core.Exceptions;

public class EntityNotFoundException<T, TKey> : Exception
    where T : Entity<TKey>
    where TKey : IEquatable<TKey>, IComparable<TKey>
{
    public TKey Id { get; }

    public EntityNotFoundException(TKey id) : base($"{typeof(T).Name} with id {id} was not found.")
    {
        Id = id;
    }
}