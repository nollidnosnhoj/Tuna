namespace Audiochan.Core.Common.Interfaces
{
    public interface IResourceModel<out TKey>
    {
        TKey Id { get; }
    }
}