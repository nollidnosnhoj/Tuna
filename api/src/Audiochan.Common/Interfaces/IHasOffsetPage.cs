namespace Audiochan.Common.Interfaces
{
    public interface IHasOffsetPage
    {
        public int Offset { get; init; }
        public int Size { get; init; }
    }
}