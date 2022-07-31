using System;

namespace Audiochan.Core.Interfaces
{
    public interface IHasCursorPage<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        TKey Cursor { get; init; }
        int Size { get; init; }
    }
}