using System.Collections.Generic;

namespace Audiochan.Core.Common.Models.Responses
{
    public record CursorList<TItem>(IReadOnlyList<TItem> Items, string Next)
    {
    }
}