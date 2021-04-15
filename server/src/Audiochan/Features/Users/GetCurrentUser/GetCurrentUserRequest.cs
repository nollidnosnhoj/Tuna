using Audiochan.Core.Models.ViewModels;
using MediatR;

namespace Audiochan.Features.Users.GetCurrentUser
{
    public record GetCurrentUserRequest : IRequest<CurrentUserViewModel>
    {
    }
}