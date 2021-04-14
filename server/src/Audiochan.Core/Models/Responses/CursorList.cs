using System.Collections.Generic;

namespace Audiochan.Core.Models.Responses
{
    public record CursorList<TItem, TCursor>(IReadOnlyList<TItem> Items, TCursor Previous, TCursor Next)
    {
    }
}