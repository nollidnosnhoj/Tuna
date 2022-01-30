using System;
using Audiochan.Application.Commons.Interfaces;

namespace Audiochan.API.Models
{
    public record CursorPaginationQueryParams<TKey>(TKey Cursor, int Size) : IHasCursorPage<TKey> 
        where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        
    }
}