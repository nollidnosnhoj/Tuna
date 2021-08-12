using System.Collections.Generic;

namespace Audiochan.Core.Common.Models
{
    public record OffsetPagedListDto<TItem>(IList<TItem> Items, int? Next, int Size)
    {
        
    }
}