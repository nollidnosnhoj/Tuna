using System.Collections.Generic;

namespace Audiochan.Core.Common.Models.Responses
{
    public record CursorListDto<TItem>(IReadOnlyList<TItem> Items, string? Next)
    {
    }
}