using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Auth.Login;
using Audiochan.Core.Services;
using MediatR;

namespace Audiochan.Core.Features.Auth.Refresh
{
    public record RefreshTokenCommand : IRequest<Result<LoginSuccessViewModel>>
    {
        public string RefreshToken { get; init; } = null!;
    }

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<LoginSuccessViewModel>>
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IUnitOfWork _unitOfWork;

        public RefreshTokenCommandHandler(ITokenProvider tokenProvider, IUnitOfWork unitOfWork)
        {
            _tokenProvider = tokenProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<LoginSuccessViewModel>> Handle(RefreshTokenCommand command,
            CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users
                .LoadForRefreshToken(command.RefreshToken, cancellationToken);

            if (user == null)
                return Result<LoginSuccessViewModel>.Fail(ResultError.BadRequest,
                    "Refresh token does not belong to a user.");

            var existingRefreshToken = user.RefreshTokens
                .Single(r => r.Token == command.RefreshToken);

            if (!await _tokenProvider.ValidateRefreshToken(existingRefreshToken.Token))
                return Result<LoginSuccessViewModel>.Fail(ResultError.BadRequest,
                    "Refresh token is invalid/expired.");

            var (refreshToken, refreshTokenExpiration) =
                await _tokenProvider.GenerateRefreshToken(user, existingRefreshToken.Token);
            var (token, tokenExpiration) = await _tokenProvider.GenerateAccessToken(user);

            return Result<LoginSuccessViewModel>.Success(new LoginSuccessViewModel
            {
                AccessToken = token,
                AccessTokenExpires = tokenExpiration,
                RefreshToken = refreshToken,
                RefreshTokenExpires = refreshTokenExpiration
            });
        }
    }
}