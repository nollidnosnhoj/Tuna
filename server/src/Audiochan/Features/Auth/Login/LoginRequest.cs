using Audiochan.Core.Models.Interfaces;
using Audiochan.Core.Models.ViewModels;
using MediatR;

namespace Audiochan.Features.Auth.Login
{
    public record LoginRequest : IRequest<IResult<AuthResultViewModel>>
    {
        public string Login { get; init; }
        public string Password { get; init; }
    }
}