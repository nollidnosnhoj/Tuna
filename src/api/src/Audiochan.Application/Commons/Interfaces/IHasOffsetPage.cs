namespace Audiochan.Application.Commons.Interfaces
{
    public interface IHasOffsetPage
    {
        public int Offset { get; init; }
        public int Size { get; init; }
    }
}