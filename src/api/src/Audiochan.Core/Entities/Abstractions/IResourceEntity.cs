namespace Audiochan.Core.Entities.Abstractions
{
    public interface IResourceEntity<out TKey>
    {
        TKey Id { get; }
    }
}