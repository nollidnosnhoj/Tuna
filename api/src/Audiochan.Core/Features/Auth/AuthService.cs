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

        public async Task<IResult<AuthResultDto>> Login(string username, string password, 
            CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByNameAsync(username.ToLower());

            if (user == null || !await _userManager.CheckPasswordAsync(user, password))
                return Result<AuthResultDto>.Fail(ResultErrorCode.UnprocessedEntity, "Invalid username/password.");

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
                UserName = request.Username.ToLower(),
                DisplayName = request.Username,
                Email = request.Email,
                Created = _dateTimeService.Now,
                Profile = new Profile()
            };

            var identityResult = await _userManager.CreateAsync(user, request.Password);

            if (!identityResult.Succeeded) return identityResult.ToResult();

            return Result.Success();
        }

        public async Task<IResult<AuthResultDto>> Refresh(string? refreshToken, 
            CancellationToken cancellationToken = default)
        {
            // Fail when refresh token is not defined
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Result<AuthResultDto>.Fail(ResultErrorCode.BadRequest, "Refresh token was not defined.");
            }
            
            // get the user and his/her refresh tokens based on the defined refresh token
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens
                    .Any(t => t.Token == refreshToken && t.UserId == u.Id), cancellationToken);

            if (user == null)
            {
                return Result<AuthResultDto>.Fail(ResultErrorCode.BadRequest, 
                    "Refresh token does not belong to a user.");
            }
            
            var existingRefreshToken = user.RefreshTokens
                .SingleOrDefault(r => r.Token == refreshToken);

            if (existingRefreshToken == null)
            {
                return Result<AuthResultDto>.Fail(ResultErrorCode.BadRequest, 
                    "Refresh token does not belong to a user.");
            }

            if (!_tokenService.IsRefreshTokenValid(existingRefreshToken))
            {
                return Result<AuthResultDto>.Fail(ResultErrorCode.BadRequest, 
                    "Refresh token is invalid/expired.");
            }

            // Create new refresh token
            // Revoked the old token (defined token)
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

        public async Task<IResult> Revoke(string? refreshToken, CancellationToken cancellationToken = default)
        {
            // Fail when refresh token is not defined
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Result.Fail(ResultErrorCode.BadRequest, "Refresh token was not defined.");
            }
            
            var user = await _userManager.Users
                .Include(u => u.RefreshTokens)
                .SingleOrDefaultAsync(u => u.RefreshTokens
                    .Any(r => r.Token == refreshToken && u.Id == r.UserId), cancellationToken);

            if (user == null)
            {
                return Result.Fail(ResultErrorCode.BadRequest, "Refresh token does not belong to a user.");
            }

            var existingRefreshToken = user.RefreshTokens
                .SingleOrDefault(r => r.Token == refreshToken);

            if (existingRefreshToken == null)
            {
                return Result.Fail(ResultErrorCode.BadRequest, "Refresh token does not belong to a user.");
            }

            // If the token is already revoked/invalid, just return success.
            // If not, then update refresh token.
            if (_tokenService.IsRefreshTokenValid(existingRefreshToken))
            {
                existingRefreshToken.Revoked = _dateTimeService.Now;
                await _userManager.UpdateAsync(user);
            }

            return Result.Success();
        }
    }
}