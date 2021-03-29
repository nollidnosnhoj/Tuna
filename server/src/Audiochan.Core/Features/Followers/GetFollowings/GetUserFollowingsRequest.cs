using Audiochan.Core.Common.Models.Requests;

namespace Audiochan.Core.Features.Followers.GetFollowings
{
    public record GetUserFollowingsRequest : PaginationQueryRequest<FollowingViewModel>
    {
        public string Username { get; init; }
    }
}