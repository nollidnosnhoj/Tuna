using Audiochan.Common.Interfaces;

namespace Audiochan.API.Models
{
    public record PaginationQueryParams : IHasPage
    {
        public int Page { get; init; }
        public int Size { get; init; }
    }
}