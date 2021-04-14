using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Models.ViewModels;
using Audiochan.Core.Features.Auth.Login;
using MediatR;

namespace Audiochan.Core.Features.Auth.Refresh
{
    public record RefreshTokenRequest : IRequest<IResult<AuthResultViewModel>>
    {
        public string RefreshToken { get; init; }
    }
}