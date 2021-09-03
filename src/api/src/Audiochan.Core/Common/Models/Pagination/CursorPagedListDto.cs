using System;
using System.Collections.Generic;
using System.Linq;
using Audiochan.Domain.Abstractions;

namespace Audiochan.Core.Common.Models.Pagination
{
    public class CursorPagedListDto<TItem, TKey> 
        where TItem : IHasId<TKey> 
        where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        public List<TItem> Items { get; }
        public TKey? Cursor { get; }
        public int Size { get; }
        
        public CursorPagedListDto(List<TItem> items, int size)
        {
            Items = items;
            Size = size;
            Cursor = items.Count < size ? default : items[^1].Id;
        }
    }
}