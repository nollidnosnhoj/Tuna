using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Common.Models.Requests
{
    public record PaginationQueryRequest<TResponse> : IRequest<PagedList<TResponse>>
    {
        public int Page { get; init; } = 1;
        public int Size { get; init; } = 30;
    }
}