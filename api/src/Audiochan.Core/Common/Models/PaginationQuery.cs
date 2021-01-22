namespace Audiochan.Core.Common.Models
{
    public record PaginationQuery
    {
        public int Page { get; init; } = 1;
        public int Size { get; init; } = 30;
    }
}