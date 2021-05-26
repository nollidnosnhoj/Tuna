using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using MediatR;

namespace Audiochan.Core.Features.Auth.Refresh
{
    public record RefreshTokenRequest : IRequest<IResult<AuthResultViewModel>>
    {
        public string RefreshToken { get; init; } = null!;
    }

    public class RefreshTokenRequestHandler : IRequestHandler<RefreshTokenRequest, IResult<AuthResultViewModel>>
    {
        private readonly ITokenProvider _tokenProvider;
        private readonly IUnitOfWork _unitOfWork;

        public RefreshTokenRequestHandler(ITokenProvider tokenProvider, IUnitOfWork unitOfWork)
        {
            _tokenProvider = tokenProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task<IResult<AuthResultViewModel>> Handle(RefreshTokenRequest request,
            CancellationToken cancellationToken)
        {
            var user = await _unitOfWork.Users.GetBySpecAsync(
                new GetUserBasedOnRefreshTokenSpecification(request.RefreshToken),
                cancellationToken: cancellationToken);

            if (user == null)
                return Result<AuthResultViewModel>.Fail(ResultError.BadRequest,
                    "Refresh token does not belong to a user.");

            var existingRefreshToken = user.RefreshTokens
                .Single(r => r.Token == request.RefreshToken);

            if (!await _tokenProvider.ValidateRefreshToken(existingRefreshToken.Token))
                return Result<AuthResultViewModel>.Fail(ResultError.BadRequest,
                    "Refresh token is invalid/expired.");

            var (refreshToken, refreshTokenExpiration) =
                await _tokenProvider.GenerateRefreshToken(user, existingRefreshToken.Token);
            var (token, tokenExpiration) = await _tokenProvider.GenerateAccessToken(user);

            return Result<AuthResultViewModel>.Success(new AuthResultViewModel
            {
                AccessToken = token,
                AccessTokenExpires = tokenExpiration,
                RefreshToken = refreshToken,
                RefreshTokenExpires = refreshTokenExpiration
            });
        }
    }
}