using System;

namespace Audiochan.Application.Commons.Interfaces
{
    public interface IHasCursorPage<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        TKey Cursor { get; init; }
        int Size { get; init; }
    }
}