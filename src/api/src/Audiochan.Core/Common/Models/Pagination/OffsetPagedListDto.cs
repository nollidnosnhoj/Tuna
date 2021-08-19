using System.Collections.Generic;

namespace Audiochan.Core.Common.Models.Pagination
{
    public record OffsetPagedListDto<TItem>(IList<TItem> Items, int? Next, int Size)
    {
        
    }
}