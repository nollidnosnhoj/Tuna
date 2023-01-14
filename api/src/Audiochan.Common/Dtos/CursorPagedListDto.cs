namespace Audiochan.Common.Dtos
{
    public class CursorPagedListDto<TItem, TKey> 
        where TItem : class
        where TKey : IEquatable<TKey>, IComparable<TKey>
    {
        public List<TItem> Items { get; }
        public TKey? Cursor { get; }
        public int Size { get; }
        
        public CursorPagedListDto(List<TItem> items, int size, Func<List<TItem>, TKey> cursorFunc)
        {
            Items = items;
            Size = size;
            Cursor = items.Count < size ? default : cursorFunc(items);
        }
    }
}