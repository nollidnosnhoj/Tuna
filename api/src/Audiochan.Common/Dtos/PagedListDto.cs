namespace Audiochan.Common.Dtos
{
    public record PagedListDto<T>
    {
        public IList<T> Items { get; }
        public int Count { get; }
        public int Page { get; }
        public int Size { get; }
        public int TotalPages { get; }
        public bool HasPrevious { get; }
        public bool HasNext { get; }

        public PagedListDto(IList<T> items, int count, int page, int size)
        {
            Items = items;
            Count = count;
            Page = page;
            Size = size;
            TotalPages = (int)Math.Ceiling(count / (double)size);
            HasPrevious = page > 1;
            HasNext = Page < TotalPages;
        }
    }
}