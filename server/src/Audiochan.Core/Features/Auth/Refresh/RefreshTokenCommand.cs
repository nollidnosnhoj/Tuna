using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Auth.Login;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Auth.Refresh
{
    public record RefreshTokenCommand : IRequest<IResult<AuthResultViewModel>>
    {
        public string RefreshToken { get; init; }
    }

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, IResult<AuthResultViewModel>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IDateTimeService _dateTimeService;

        public RefreshTokenCommandHandler(UserManager<User> userManager, ITokenService tokenService,
            IDateTimeService dateTimeService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _dateTimeService = dateTimeService;
        }

        public async Task<IResult<AuthResultViewModel>> Handle(RefreshTokenCommand request,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.RefreshToken))
                return Result<AuthResultViewModel>
                    .Fail(ResultError.BadRequest, "Refresh token was not defined.");

            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens
                    .Any(t => t.Token == request.RefreshToken && t.UserId == u.Id), cancellationToken);

            if (user == null)
                return Result<AuthResultViewModel>.Fail(ResultError.BadRequest,
                    "Refresh token does not belong to a user.");

            var existingRefreshToken = user.RefreshTokens
                .Single(r => r.Token == request.RefreshToken);

            if (!_tokenService.IsRefreshTokenValid(existingRefreshToken))
                return Result<AuthResultViewModel>.Fail(ResultError.BadRequest,
                    "Refresh token is invalid/expired.");

            var newRefreshToken = _tokenService.GenerateRefreshToken(user.Id);
            existingRefreshToken.Revoked = _dateTimeService.Now;
            existingRefreshToken.ReplacedByToken = newRefreshToken.Token;
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var (token, tokenExpiration) = await _tokenService.GenerateAccessToken(user);

            return Result<AuthResultViewModel>.Success(new AuthResultViewModel
            {
                AccessToken = token,
                AccessTokenExpires = tokenExpiration,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpires = _tokenService.DateTimeToUnixEpoch(newRefreshToken.Expiry)
            });
        }
    }
}