using System;
using Audiochan.Domain.Abstractions;

namespace Audiochan.Application.Commons.Exceptions;

public class NotFoundException : BadRequestException
{
    public NotFoundException() : base("Resource was not found.")
    {
        
    }
}

public class NotFoundException<T, TKey> : NotFoundException
    where T : IHasId<TKey>
    where TKey : IEquatable<TKey>, IComparable<TKey>
{
    public Type Type { get; }
    public TKey Id { get; }

    public NotFoundException(TKey id)
    {
        Type = typeof(T);
        Id = id;
    }
}