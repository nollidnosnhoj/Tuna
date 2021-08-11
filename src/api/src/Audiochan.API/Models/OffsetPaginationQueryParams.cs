using Audiochan.Core.Common.Interfaces;

namespace Audiochan.API.Models
{
    public record OffsetPaginationQueryParams(int Offset, int Size) : IHasOffsetPage;
}