namespace Audiochan.Domain.Abstractions
{
    public interface IResourceEntity<out TKey>
    {
        TKey Id { get; }
    }
}