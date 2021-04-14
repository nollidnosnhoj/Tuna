using Audiochan.Core.Models.Interfaces;
using MediatR;

namespace Audiochan.API.Features.Followers.SetFollow
{
    public record SetFollowRequest(string UserId, string Username, bool IsFollowing) : IRequest<IResult<bool>>
    {
    }
}