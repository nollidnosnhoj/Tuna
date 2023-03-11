using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core;
using Audiochan.Core.Features.Auth;
using Audiochan.Core.Features.Auth.Errors;
using Audiochan.Core.Features.Auth.Models;
using Audiochan.Core.Features.Auth.Results;
using Audiochan.Core.Persistence;
using Audiochan.Core.Services;
using Audiochan.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Audiochan.Infrastructure.Identity;

public class IdentityAuthService : IAuthService
{
    private readonly UserManager<AudiochanIdentityUser> _userManager;
    private readonly ITokenProvider _tokenProvider;
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly IDateTimeProvider _dateTimeProvider;

    public IdentityAuthService(UserManager<AudiochanIdentityUser> userManager, ITokenProvider tokenProvider, IDbContextFactory<ApplicationDbContext> dbContextFactory, IDateTimeProvider dateTimeProvider)
    {
        _userManager = userManager;
        _tokenProvider = tokenProvider;
        _dbContextFactory = dbContextFactory;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<LoginResult> LoginWithPasswordAsync(string login, string password, string? ipAddress = null, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByNameAsync(login);

        if (user is null)
        {
            return new LoginWithPasswordFailed();
        }

        await EnsurePasswordIsValidAsync(user, password);

        var (token, expirationDate) = await GenerateAccessToken(user, cancellationToken);
        var refreshToken = _tokenProvider.GenerateRefreshToken();
        user.RefreshTokens.Add(new RefreshToken(refreshToken, user.Id, ipAddress, _dateTimeProvider.Now, _dateTimeProvider.Now.AddDays(7)));
        await _userManager.UpdateAsync(user);

        return new AuthTokenResult(token, refreshToken, expirationDate);
    }

    public async Task<RefreshTokenResult> RefreshTokenAsync(string refreshToken, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindUserByRefreshTokenAsync(refreshToken);
        if (user is null) return new IdentityUserNotFound();
        var newRefreshToken = _tokenProvider.GenerateRefreshToken();
        var refreshTokenModel = user.RefreshTokens.First(x => x.Token == refreshToken);
        if (!refreshTokenModel.IsActive) return new RefreshTokenExpired();
        refreshTokenModel.Revoke(_dateTimeProvider.Now, ipAddress, "Refresh token");
        refreshTokenModel.ReplacedByToken = newRefreshToken;
        user.RefreshTokens.Add(new RefreshToken(newRefreshToken, user.Id, ipAddress, _dateTimeProvider.Now, _dateTimeProvider.Now.AddDays(7)));
        await _userManager.UpdateAsync(user);
        var (token, expirationDate) = await GenerateAccessToken(user, cancellationToken);
        return new AuthTokenResult(token, newRefreshToken, expirationDate);
    }

    public async Task<RevokeRefreshTokenResult> RevokeRefreshTokenAsync(string refreshToken, string? ipAddress, CancellationToken? cancellationToken = default)
    {
        var user = await _userManager.FindUserByRefreshTokenAsync(refreshToken);
        if (user is null) return new IdentityUserNotFound();
        var refreshTokenModel = user.RefreshTokens.First(x => x.Token == refreshToken);
        if (!refreshTokenModel.IsActive) return false;
        refreshTokenModel.Revoke(_dateTimeProvider.Now, ipAddress, "Log out");
        await _userManager.UpdateAsync(user);
        return true;
    }

    private async Task<(string token, DateTime expirationDate)> GenerateAccessToken(AudiochanIdentityUser user, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var appUser = await dbContext.Users.FirstOrDefaultAsync(x => x.IdentityId == user.Id, cancellationToken);
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(ClaimNames.HasProfile, (appUser is not null).ToString())
        };
        
        if (appUser is not null)
        {
            claims.Add(new Claim(ClaimNames.UserId, appUser.Id.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Name, appUser.UserName));
        }
        var expiration = _dateTimeProvider.Now.AddMinutes(30);
        var token = _tokenProvider.GenerateAccessToken(claims, expiration);
        return (token, expiration);
    }

    private async Task EnsurePasswordIsValidAsync(AudiochanIdentityUser user, string password)
    {
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);

        if (!isPasswordValid)
        {
            // TODO: Create custom exception for invalid login.
            throw new UnauthorizedAccessException();
        }
    }
}