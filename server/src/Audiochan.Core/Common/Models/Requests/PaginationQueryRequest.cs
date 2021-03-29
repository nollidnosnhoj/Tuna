namespace Audiochan.Core.Common.Models.Requests
{
    public record PaginationQueryRequest
    {
        public int Page { get; init; } = 1;
        public int Size { get; init; } = 30;
    }
}