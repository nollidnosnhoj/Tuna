using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Features.Followers.SetFollow
{
    public record SetFollowRequest(string UserId, string Username, bool IsFollowing) : IRequest<IResult<bool>>
    {
    }
}