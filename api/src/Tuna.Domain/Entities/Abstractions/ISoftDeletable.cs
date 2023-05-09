namespace Tuna.Domain.Entities.Abstractions;

public interface ISoftDeletable
{
    public DateTime? Deleted { get; set; }
}