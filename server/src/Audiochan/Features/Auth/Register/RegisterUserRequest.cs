using Audiochan.Core.Models.Interfaces;
using MediatR;

namespace Audiochan.Features.Auth.Register
{
    public class RegisterUserRequest : IRequest<IResult<bool>>
    {
        public string Username { get; init; } = string.Empty;
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
}