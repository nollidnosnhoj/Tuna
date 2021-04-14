using Audiochan.Core.Models.ViewModels;
using MediatR;

namespace Audiochan.API.Features.Users.GetUser
{
    public record GetUserRequest(string Username) : IRequest<UserViewModel>
    {
    }
}