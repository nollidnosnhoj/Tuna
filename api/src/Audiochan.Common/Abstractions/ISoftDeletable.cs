namespace Audiochan.Common.Abstractions
{
    public interface ISoftDeletable
    {
        public DateTime? Deleted { get; set; }
    }
}