using Audiochan.Application.Commons.Interfaces;

namespace Audiochan.API.Models
{
    public record OffsetPaginationQueryParams(int Offset = 0, int Size = 30) : IHasOffsetPage;
}