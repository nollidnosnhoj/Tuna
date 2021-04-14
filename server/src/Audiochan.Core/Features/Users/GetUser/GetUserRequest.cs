using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Models.ViewModels;
using MediatR;

namespace Audiochan.Core.Features.Users.GetUser
{
    public record GetUserRequest(string Username) : IRequest<UserViewModel>
    {
    }
}