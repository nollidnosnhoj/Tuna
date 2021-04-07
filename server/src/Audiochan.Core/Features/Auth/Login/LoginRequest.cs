using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Features.Auth.Login
{
    public record LoginRequest : IRequest<IResult<AuthResultViewModel>>
    {
        public string Login { get; init; }
        public string Password { get; init; }
    }
}