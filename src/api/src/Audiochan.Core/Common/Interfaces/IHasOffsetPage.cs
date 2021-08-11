namespace Audiochan.Core.Common.Interfaces
{
    public interface IHasOffsetPage
    {
        public int Offset { get; init; }
        public int Size { get; init; }
    }
}