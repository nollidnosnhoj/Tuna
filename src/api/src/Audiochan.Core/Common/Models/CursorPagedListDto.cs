using System.Collections.Generic;

namespace Audiochan.Core.Common.Models
{
    public record CursorPagedListDto<TItem>(IList<TItem> Items, int? NextCursor, int Size)
    {
        
    }
}