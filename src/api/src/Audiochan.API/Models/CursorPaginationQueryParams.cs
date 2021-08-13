using Audiochan.Core.Common.Interfaces;

namespace Audiochan.API.Models
{
    public record CursorPaginationQueryParams<TKey>(TKey? Cursor, int Size) : IHasCursorPage<TKey> where TKey : struct
    {
        
    }
}