using MediatR;

namespace Audiochan.Features.Followers.CheckIfFollowing
{
    public record CheckIfUserIsFollowingRequest(string UserId, string Username) : IRequest<bool>
    {
    }
}