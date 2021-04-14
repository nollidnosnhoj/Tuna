using Audiochan.Core.Models.ViewModels;
using MediatR;

namespace Audiochan.API.Features.Users.GetCurrentUser
{
    public record GetCurrentUserRequest : IRequest<CurrentUserViewModel>
    {
    }
}