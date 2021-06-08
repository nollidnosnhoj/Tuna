namespace Audiochan.Core.Common.Interfaces
{
    public interface IInvalidateCache
    {
        public string[] InvalidCacheKeys { get; }
    }
}