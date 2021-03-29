using Audiochan.Core.Common.Models.Requests;

namespace Audiochan.Core.Features.Followers.GetFollowers
{
    public record GetUserFollowersRequest : PaginationQueryRequest<FollowerViewModel>
    {
        public string Username { get; init; }
    }
}