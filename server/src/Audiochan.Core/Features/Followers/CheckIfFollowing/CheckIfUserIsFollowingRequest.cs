using MediatR;

namespace Audiochan.Core.Features.Followers.CheckIfFollowing
{
    public record CheckIfUserIsFollowingRequest(string UserId, string Username) : IRequest<bool>
    {
    }
}