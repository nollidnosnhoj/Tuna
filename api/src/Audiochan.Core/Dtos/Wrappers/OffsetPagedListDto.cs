using System.Collections.Generic;

namespace Audiochan.Core.Dtos.Wrappers
{
    public class OffsetPagedListDto<TItem>
    {
        public IList<TItem> Items { get; }
        public int? Next { get; }
        public int Size { get; }

        public OffsetPagedListDto(IList<TItem> items, int offset, int size)
        {
            Items = items;
            Size = size;
            Next = Items.Count == size
                ? offset + size
                : null;
        }
    }
}