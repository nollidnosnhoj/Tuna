namespace Audiochan.Core.Common.Interfaces
{
    public interface IHasCursor
    {
        long? Cursor { get; init; }
        int? Size { get; init; }
    }
}