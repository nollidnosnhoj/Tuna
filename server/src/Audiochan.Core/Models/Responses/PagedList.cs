using System;
using System.Collections.Generic;

namespace Audiochan.Core.Models.Responses
{
    public record PagedList<T>(List<T> Items, int Count, int Page, int Size)
    {
        public int TotalPages => (int) Math.Ceiling(Count / (double) Size);
        public bool HasPrevious => Page > 1;
        public bool HasNext => Page < TotalPages;
    }
}