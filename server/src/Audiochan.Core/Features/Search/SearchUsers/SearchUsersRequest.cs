using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Features.Users.GetUser;
using MediatR;

namespace Audiochan.Core.Features.Search.SearchUsers
{
    public record SearchUsersRequest : PaginationQueryRequest, IRequest<PagedList<UserViewModel>>
    {
        public string Q { get; init; }
    }
}