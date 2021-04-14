using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Models.Interfaces;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Models.ViewModels;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Auth.Login;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Auth.Refresh
{
    public class RefreshTokenRequestHandler : IRequestHandler<RefreshTokenRequest, IResult<AuthResultViewModel>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenProvider _tokenProvider;
        private readonly IDateTimeProvider _dateTimeProvider;

        public RefreshTokenRequestHandler(UserManager<User> userManager, ITokenProvider tokenProvider,
            IDateTimeProvider dateTimeProvider)
        {
            _userManager = userManager;
            _tokenProvider = tokenProvider;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<IResult<AuthResultViewModel>> Handle(RefreshTokenRequest request,
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

            if (existingRefreshToken == null || existingRefreshToken.Expiry <= _dateTimeProvider.Now)
                return Result<AuthResultViewModel>.Fail(ResultError.BadRequest,
                    "Refresh token is invalid/expired.");

            var newRefreshToken = _tokenProvider.GenerateRefreshToken(user.Id);
            user.RefreshTokens.Remove(existingRefreshToken);
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var (token, tokenExpiration) = await _tokenProvider.GenerateAccessToken(user);

            return Result<AuthResultViewModel>.Success(new AuthResultViewModel
            {
                AccessToken = token,
                AccessTokenExpires = tokenExpiration,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpires = _tokenProvider.DateTimeToUnixEpoch(newRefreshToken.Expiry)
            });
        }
    }
}