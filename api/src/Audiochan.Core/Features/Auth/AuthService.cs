using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Extensions;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Features.Auth.Models;
using Audiochan.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Auth
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IDateTimeService _dateTimeService;

        public AuthService(UserManager<User> userManager, 
            ITokenService tokenService, 
            IDateTimeService dateTimeService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _dateTimeService = dateTimeService;
        }

        public async Task<IResult<AuthResultDto>> Login(string login, string password, 
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.Users
                .SingleOrDefaultAsync(u => u.UserName == login.ToLower() || u.Email == login, cancellationToken);

            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                return Result<AuthResultDto>.Fail(ResultStatus.Unauthorized);

            var token = await _tokenService.GenerateAccessToken(user);

            var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

            user.RefreshTokens.Add(refreshToken);
            await _userManager.UpdateAsync(user);

            var result = new AuthResultDto
            {
                AccessToken = token,
                RefreshToken = refreshToken.Token,
                RefreshTokenExpires = _tokenService.DateTimeToUnixEpoch(refreshToken.Expiry)
            };

            return Result<AuthResultDto>.Success(result);
        }

        public async Task<IResult> Register(CreateUserRequest request, CancellationToken cancellationToken = default)
        {
            var user = new User
            {
                UserName = request.Username!.ToLower(),
                DisplayName = request.Username,
                Email = request.Email,
                Joined = _dateTimeService.Now
            };

            var identityResult = await _userManager.CreateAsync(user, request.Password);

            return identityResult.Succeeded
                ? Result.Success()
                : identityResult.ToResult();
        }

        public async Task<IResult<AuthResultDto>> Refresh(string refreshToken, 
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return Result<AuthResultDto>
                    .Fail(ResultStatus.BadRequest, "Refresh token was not defined.");
            
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens
                    .Any(t => t.Token == refreshToken && t.UserId == u.Id), cancellationToken);

            if (user == null)
                return Result<AuthResultDto>.Fail(ResultStatus.BadRequest, 
                    "Refresh token does not belong to a user.");
            
            var existingRefreshToken = user.RefreshTokens
                .Single(r => r.Token == refreshToken);

            if (!_tokenService.IsRefreshTokenValid(existingRefreshToken))
                return Result<AuthResultDto>.Fail(ResultStatus.BadRequest, 
                    "Refresh token is invalid/expired.");
            
            var newRefreshToken = _tokenService.GenerateRefreshToken(user.Id);
            existingRefreshToken.Revoked = _dateTimeService.Now;
            existingRefreshToken.ReplacedByToken = newRefreshToken.Token;
            user.RefreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var accessToken = await _tokenService.GenerateAccessToken(user);

            return Result<AuthResultDto>.Success(new AuthResultDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token,
                RefreshTokenExpires = _tokenService.DateTimeToUnixEpoch(newRefreshToken.Expiry)
            });
        }

        public async Task<IResult> Revoke(string refreshToken, CancellationToken cancellationToken = default)
        {
            // Fail when refresh token is not defined
            if (string.IsNullOrEmpty(refreshToken))
                return Result.Fail(ResultStatus.BadRequest, "Refresh token was not defined.");
            
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens
                    .Any(r => r.Token == refreshToken && u.Id == r.UserId), cancellationToken);

            if (user == null)
                return Result.Fail(ResultStatus.BadRequest, "Refresh token does not belong to a user.");

            var existingRefreshToken = user.RefreshTokens
                .Single(r => r.Token == refreshToken);
            
            if (_tokenService.IsRefreshTokenValid(existingRefreshToken))
            {
                existingRefreshToken.Revoked = _dateTimeService.Now;
                await _userManager.UpdateAsync(user);
            }

            return Result.Success();
        }
    }
}