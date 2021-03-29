using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Features.Users.GetUser
{
    public record GetUserRequest(string Username) : IRequest<IResult<UserViewModel>>
    {
    }
}