using Audiochan.Core.Models.ViewModels;
using MediatR;

namespace Audiochan.Core.Features.Users.GetCurrentUser
{
    public record GetCurrentUserRequest : IRequest<CurrentUserViewModel>
    {
    }
}