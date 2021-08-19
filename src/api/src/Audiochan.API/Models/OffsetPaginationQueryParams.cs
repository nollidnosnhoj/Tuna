using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Interfaces.Pagination;

namespace Audiochan.API.Models
{
    public record OffsetPaginationQueryParams(int Offset, int Size) : IHasOffsetPage;
}