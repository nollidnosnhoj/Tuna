using System;

namespace Audiochan.Domain.Abstractions
{
    public interface IHasId<out TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        TKey Id { get; }
    }
}