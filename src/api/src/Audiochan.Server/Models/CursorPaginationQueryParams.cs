using System;
using Audiochan.Application.Commons.Interfaces;

namespace Audiochan.Server.Models
{
    public record CursorPaginationQueryParams<TKey>(TKey Cursor, int Size) : IHasCursorPage<TKey> 
        where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        
    }
}