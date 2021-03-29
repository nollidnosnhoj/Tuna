using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Features.Followers.GetFollowers
{
    public record GetUserFollowersRequest : PaginationQueryRequest, IRequest<PagedList<FollowerViewModel>>
    {
        public string Username { get; init; }
    }
}