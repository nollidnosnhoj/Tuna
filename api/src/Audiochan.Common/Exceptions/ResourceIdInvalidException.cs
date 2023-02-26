namespace Audiochan.Common.Exceptions;

public class ResourceIdInvalidException<TKey> : Exception
    where TKey : IEquatable<TKey>, IComparable<TKey>
{
    public string Resource { get; }
    public TKey Id { get; }
    
    public ResourceIdInvalidException(Type resourceType, TKey id)
        : base($"{resourceType.Name} with id: ({id}) is invalid.")
    {
        Resource = resourceType.Name;
        Id = id;
    }
}