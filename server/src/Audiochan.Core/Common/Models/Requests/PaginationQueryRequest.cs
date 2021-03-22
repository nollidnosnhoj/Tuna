using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Common.Models.Requests
{
    public record PaginationQueryRequest<TResponse> : PaginationQuery, IRequest<PagedList<TResponse>>
    {
    }
}