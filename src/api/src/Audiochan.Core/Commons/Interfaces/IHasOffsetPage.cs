namespace Audiochan.Core.Commons.Interfaces
{
    public interface IHasOffsetPage
    {
        public int Offset { get; init; }
        public int Size { get; init; }
    }
}