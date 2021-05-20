namespace Audiochan.Core.Common.Models.Interfaces
{
    public interface IHasCursor
    {
        string? Cursor { get; init; }
        int Size { get; init; }
    }
}