namespace Audiochan.Core.Common.Interfaces
{
    public interface IResourceDto<out TKey>
    {
        TKey Id { get; }
    }
}