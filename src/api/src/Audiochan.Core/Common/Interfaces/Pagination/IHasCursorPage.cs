using System;

namespace Audiochan.Core.Common.Interfaces.Pagination
{
    public interface IHasCursorPage<TKey> where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        TKey Cursor { get; init; }
        int Size { get; init; }
    }
}