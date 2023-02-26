namespace Audiochan.Common.Exceptions;

public class ResourceOwnershipException<TKey> : Exception
    where TKey : IEquatable<TKey>, IComparable<TKey>
{
    public long UserId { get; }
    public TKey Id { get; }
    public string Resource { get; }

    public ResourceOwnershipException(Type resourceType, TKey resourceId, long userId)
        : base("User forbids from modifying this resource.")
    {
        Resource = resourceType.Name;
        Id = resourceId;
        UserId = userId;
    }
}