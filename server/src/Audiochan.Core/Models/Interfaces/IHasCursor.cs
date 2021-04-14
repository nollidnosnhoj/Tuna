namespace Audiochan.Core.Models.Interfaces
{
    public interface IHasCursor
    {
        long? Cursor { get; init; }
        int? Size { get; init; }
    }
}