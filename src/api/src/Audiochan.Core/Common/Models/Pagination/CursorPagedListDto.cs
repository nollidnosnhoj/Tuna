using System.Collections.Generic;

namespace Audiochan.Core.Common.Models.Pagination
{
    public record CursorPagedListDto<TItem>(IList<TItem> Items, long? Next, int Size)
    {
        
    }
}