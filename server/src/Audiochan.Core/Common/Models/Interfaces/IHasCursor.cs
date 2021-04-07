namespace Audiochan.Core.Common.Models.Interfaces
{
    public interface IHasCursor
    {
        long? Cursor { get; init; }
        int? Size { get; init; }
    }
}