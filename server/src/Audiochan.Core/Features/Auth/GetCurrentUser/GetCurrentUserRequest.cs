using MediatR;

namespace Audiochan.Core.Features.Auth.GetCurrentUser
{
    public record GetCurrentUserRequest : IRequest<CurrentUserViewModel>
    {
    }
}