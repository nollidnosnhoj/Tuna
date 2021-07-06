using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Auth.Login
{
    public record LoginCommand : IRequest<Result<AuthResult>>
    {
        public string Login { get; init; } = null!;
        public string Password { get; init; } = null!;
    }


    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResult>>
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IIdentityService _identityService;

        public LoginCommandHandler(ITokenProvider tokenProvider, IIdentityService identityService)
        {
            _tokenProvider = tokenProvider;
            _identityService = identityService;
        }

        public async Task<Result<AuthResult>> Handle(LoginCommand command,
            CancellationToken cancellationToken)
        {
            var user = await _identityService.LoginUserAsync(command.Login, command.Password, cancellationToken);

            if (user == null)
                return Result<AuthResult>.BadRequest("Invalid Username/Password");

            var (accessToken, accessTokenExpiration) = await _tokenProvider.GenerateAccessToken(user);

            var (refreshToken, refreshTokenExpiration) = await _tokenProvider.GenerateRefreshToken(user);

            var result = new AuthResult
            {
                AccessToken = accessToken,
                AccessTokenExpires = accessTokenExpiration,
                RefreshToken = refreshToken,
                RefreshTokenExpires = refreshTokenExpiration
            };

            return Result<AuthResult>.Success(result);
        }
    }
}