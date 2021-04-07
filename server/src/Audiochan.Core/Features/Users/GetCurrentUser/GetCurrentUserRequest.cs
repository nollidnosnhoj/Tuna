using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Features.Users.GetCurrentUser
{
    public record GetCurrentUserRequest : IRequest<CurrentUserViewModel>
    {
    }
}