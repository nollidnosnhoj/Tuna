using Audiochan.Core.Common.Models.Requests;
using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Features.Followers.GetFollowings
{
    public record GetUserFollowingsRequest : PaginationQueryRequest, IRequest<PagedList<FollowingViewModel>>
    {
        public string Username { get; init; }
    }
}