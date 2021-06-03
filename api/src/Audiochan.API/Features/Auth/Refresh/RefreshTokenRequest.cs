using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Features.Auth.Login;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Models;
using Audiochan.Core.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.API.Features.Auth.Refresh
{
    public record RefreshTokenRequest : IRequest<Result<LoginSuccessViewModel>>
    {
        public string RefreshToken { get; init; } = null!;
    }

    public class RefreshTokenRequestHandler : IRequestHandler<RefreshTokenRequest, Result<LoginSuccessViewModel>>
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IUnitOfWork _unitOfWork;

        public RefreshTokenRequestHandler(ITokenProvider tokenProvider, IUnitOfWork unitOfWork)
        {
            _tokenProvider = tokenProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<LoginSuccessViewModel>> Handle(RefreshTokenRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens
                    .Any(t => t.Token == request.RefreshToken && t.UserId == u.Id), cancellationToken);

            if (user == null)
                return Result<LoginSuccessViewModel>.Fail(ResultError.BadRequest,
                    "Refresh token does not belong to a user.");

            var existingRefreshToken = user.RefreshTokens
                .Single(r => r.Token == request.RefreshToken);

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