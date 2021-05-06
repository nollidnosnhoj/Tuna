namespace Audiochan.Core.Common.Models.Interfaces
{
    public interface IHasCursor<TCursor> where TCursor : struct
    {
        TCursor? Cursor { get; init; }
        int Size { get; init; }
    }
}