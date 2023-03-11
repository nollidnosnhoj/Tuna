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

public class AuthService : IAuthService
{
    private readonly UserManager<AudiochanIdentityUser> _userManager;
    private readonly ITokenProvider _tokenProvider;
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AuthService(UserManager<AudiochanIdentityUser> userManager, ITokenProvider tokenProvider, IDbContextFactory<ApplicationDbContext> dbContextFactory, IDateTimeProvider dateTimeProvider)
    {
        _userManager = userManager;
        _tokenProvider = tokenProvider;
        _dbContextFactory = dbContextFactory;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<LoginResult> LoginWithPasswordAsync(string login, string password, string? ipAddress = null, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .Include(x => x.RefreshTokens)
            .Where(u => u.UserName == login)
            .SingleOrDefaultAsync(cancellationToken);

        if (user is null) return new LoginWithPasswordFailed();

        await EnsurePasswordIsValidAsync(user, password);

        var (token, expirationDate) = await GenerateAccessToken(user, cancellationToken);
        var refreshToken = new RefreshToken(
            _tokenProvider.GenerateRefreshToken(),
            ipAddress,
            _dateTimeProvider.Now,
            _dateTimeProvider.Now.AddDays(7));
        user.RefreshTokens.Add(refreshToken);
        RemoveOldRefreshTokens(user);
        await _userManager.UpdateAsync(user);

        return new AuthTokenResult(token, refreshToken.Token, expirationDate);
    }

    public async Task<RefreshTokenResult> RefreshTokenAsync(string token, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindUserByRefreshTokenAsync(token);
        if (user is null) return new IdentityUserNotFound();
        
        var refreshToken = user.RefreshTokens.First(x => x.Token == token);
        
        // user tries to use revoked token (assuming it's compromise), we need to revoke all descendant tokens
        if (refreshToken.IsRevoked)
        {
            RevokeDescendantRefreshTokens(refreshToken, user, ipAddress, $"Refresh token {token} was revoked.");
            await _userManager.UpdateAsync(user);
        }
        
        if (!refreshToken.IsActive) return new RefreshTokenExpired();

        var newRefreshToken = RotateRefreshToken(refreshToken, ipAddress);
        user.RefreshTokens.Add(newRefreshToken);
        await _userManager.UpdateAsync(user);
        
        var (accessToken, expirationDate) = await GenerateAccessToken(user, cancellationToken);
        
        return new AuthTokenResult(accessToken, newRefreshToken.Token, expirationDate);
    }

    public async Task<RevokeRefreshTokenResult> RevokeRefreshTokenAsync(string token, string? ipAddress, CancellationToken? cancellationToken = default)
    {
        var user = await _userManager.FindUserByRefreshTokenAsync(token);
        if (user is null) return new IdentityUserNotFound();
        var refreshTokenModel = user.RefreshTokens.First(x => x.Token == token);
        if (!refreshTokenModel.IsActive) return false;
        refreshTokenModel.Revoke(_dateTimeProvider.Now, ipAddress, "Log out");
        await _userManager.UpdateAsync(user);
        return true;
    }

    private RefreshToken RotateRefreshToken(RefreshToken refreshToken, string? ipAddress)
    {
        var newRefreshToken = new RefreshToken(
            _tokenProvider.GenerateRefreshToken(),
            ipAddress,
            _dateTimeProvider.Now,
            _dateTimeProvider.Now.AddDays(7));
        refreshToken.Revoke(_dateTimeProvider.Now, ipAddress, "Rotating refresh token.", newRefreshToken.Token);
        return newRefreshToken;
    }

    private void RemoveOldRefreshTokens(AudiochanIdentityUser user)
    {
        var oldRefreshTokens = user.RefreshTokens.Where(x => x.IsActive == false).ToList();
        foreach (var oldRefreshToken in oldRefreshTokens)
        {
            user.RefreshTokens.Remove(oldRefreshToken);
        }
    }

    private void RevokeDescendantRefreshTokens(RefreshToken refreshToken, AudiochanIdentityUser user, string? ipAddress, string reason)
    {
        if (string.IsNullOrEmpty(refreshToken.ReplacedByToken)) return;
        
        var childToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
        
        if (childToken is null) return;
        
        if (childToken.IsActive)
        {
            childToken.Revoke(_dateTimeProvider.Now, ipAddress, reason);
        }
        else
        {
            RevokeDescendantRefreshTokens(childToken, user, ipAddress, reason);
        }
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