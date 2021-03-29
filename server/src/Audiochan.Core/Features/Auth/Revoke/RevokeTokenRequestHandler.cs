using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Auth.Revoke
{
    public class RevokeTokenRequestHandler : IRequestHandler<RevokeTokenRequest, IResult<bool>>
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenProvider _tokenProvider;
        private readonly IDateTimeProvider _dateTimeProvider;

        public RevokeTokenRequestHandler(UserManager<User> userManager, ITokenProvider tokenProvider,
            IDateTimeProvider dateTimeProvider)
        {
            _userManager = userManager;
            _tokenProvider = tokenProvider;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<IResult<bool>> Handle(RevokeTokenRequest request, CancellationToken cancellationToken)
        {
            // Fail when refresh token is not defined
            if (string.IsNullOrEmpty(request.RefreshToken))
                return Result<bool>.Fail(ResultError.BadRequest, "Refresh token was not defined.");

            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens
                    .Any(r => r.Token == request.RefreshToken && u.Id == r.UserId), cancellationToken);

            if (user == null)
                return Result<bool>.Fail(ResultError.BadRequest, "Refresh token does not belong to a user.");

            var existingRefreshToken = user.RefreshTokens
                .Single(r => r.Token == request.RefreshToken);

            if (_tokenProvider.IsRefreshTokenValid(existingRefreshToken))
            {
                existingRefreshToken.Revoked = _dateTimeProvider.Now;
                await _userManager.UpdateAsync(user);
            }

            return Result<bool>.Success(true);
        }
    }
}