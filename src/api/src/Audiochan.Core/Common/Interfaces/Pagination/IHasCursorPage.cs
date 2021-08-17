namespace Audiochan.Core.Common.Interfaces.Pagination
{
    public interface IHasCursorPage<TKey> where TKey : struct
    {
        TKey? Cursor { get; init; }
        int Size { get; init; }
    }
}