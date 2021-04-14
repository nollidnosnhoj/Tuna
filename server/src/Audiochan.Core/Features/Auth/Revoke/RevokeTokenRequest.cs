using Audiochan.Core.Models.Interfaces;
using MediatR;

namespace Audiochan.Core.Features.Auth.Revoke
{
    public record RevokeTokenRequest : IRequest<IResult<bool>>
    {
        public string RefreshToken { get; init; }
    }
}