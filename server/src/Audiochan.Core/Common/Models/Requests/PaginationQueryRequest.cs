using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Common.Models.Requests
{
    public record PaginationQueryRequest<TResponse> : IRequest<PagedList<TResponse>>
    {
        public int Page { get; init; }
        public int Size { get; init; }
    }
}