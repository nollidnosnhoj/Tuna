using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Tuna.Application;
using Tuna.Application.Features.Auth;
using Tuna.Application.Features.Auth.Helpers;
using Tuna.Application.Features.Auth.Models;
using Tuna.Application.Features.Auth.Results;
using Tuna.Application.Persistence;
using Tuna.Application.Services;
using Tuna.Shared;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Tuna.Infrastructure.Identity.Models;

namespace Tuna.Infrastructure.Identity;

public class AuthService : IAuthService
{
    private readonly UserManager<AuthUser> _userManager;
    private readonly ITokenProvider _tokenProvider;
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AuthService(UserManager<AuthUser> userManager, ITokenProvider tokenProvider, IDbContextFactory<ApplicationDbContext> dbContextFactory, IDateTimeProvider dateTimeProvider)
    {
        _userManager = userManager;
        _tokenProvider = tokenProvider;
        _dbContextFactory = dbContextFactory;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<AuthServiceResult<AuthServiceTokens>> LoginWithPasswordAsync(string login, string password, string? ipAddress = null, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.Users
            .Include(x => x.RefreshTokens)
            .Where(u => u.UserName == login)
            .SingleOrDefaultAsync(cancellationToken);

        if (user is null)
        {
            return AuthServiceResult<AuthServiceTokens>.Failed(
                new AuthServiceError("LoginFailed","Invalid username or password."));
        }

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

        return new AuthServiceTokens(token, refreshToken.Token, expirationDate, user.ToIdentityUserDto());
    }

    public async Task<AuthServiceResult<AuthServiceTokens>> RefreshTokenAsync(string token, string? ipAddress, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindUserByRefreshTokenAsync(token);
        
        if (user is null) 
            return new AuthServiceError("RefreshTokenNotFound", "Refresh token not found.");
        
        var refreshToken = user.RefreshTokens.First(x => x.Token == token);
        
        // user tries to use revoked token (assuming it's compromise), we need to revoke all descendant tokens
        if (refreshToken.IsRevoked)
        {
            RevokeDescendantRefreshTokens(refreshToken, user, ipAddress, $"Refresh token {token} was revoked.");
            await _userManager.UpdateAsync(user);
        }
        
        if (!refreshToken.IsActive) 
            return new AuthServiceError("RefreshTokenExpired", "Refresh token expired.");

        var newRefreshToken = RotateRefreshToken(refreshToken, ipAddress);
        user.RefreshTokens.Add(newRefreshToken);
        await _userManager.UpdateAsync(user);
        
        var (accessToken, expirationDate) = await GenerateAccessToken(user, cancellationToken);
        
        return new AuthServiceTokens(accessToken, newRefreshToken.Token, expirationDate, user.ToIdentityUserDto());
    }

    public async Task<AuthServiceResult> RevokeRefreshTokenAsync(string token, string? ipAddress, CancellationToken? cancellationToken = default)
    {
        var user = await _userManager.FindUserByRefreshTokenAsync(token);
        if (user is null) return new AuthServiceError("RefreshTokenNotFound", "Refresh token not found.");
        var refreshTokenModel = user.RefreshTokens.First(x => x.Token == token);
        if (!refreshTokenModel.IsActive) return new AuthServiceError("RefreshTokenExpired", "Refresh token expired.");
        refreshTokenModel.Revoke(_dateTimeProvider.Now, ipAddress, "Log out");
        await _userManager.UpdateAsync(user);
        return AuthServiceResult.Succeed();
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

    private void RemoveOldRefreshTokens(AuthUser user)
    {
        var oldRefreshTokens = user.RefreshTokens.Where(x => x.IsActive == false).ToList();
        foreach (var oldRefreshToken in oldRefreshTokens)
        {
            user.RefreshTokens.Remove(oldRefreshToken);
        }
    }

    private void RevokeDescendantRefreshTokens(RefreshToken refreshToken, AuthUser user, string? ipAddress, string reason)
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

    private async Task<(string token, DateTime expirationDate)> GenerateAccessToken(AuthUser identityUser, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.IdentityId == identityUser.Id, cancellationToken);
        var claims = UserClaimsHelpers.ToClaims(identityUser.Id, user);
        var expiration = _dateTimeProvider.Now.AddMinutes(30);
        var token = _tokenProvider.GenerateAccessToken(claims, expiration);
        return (token, expiration);
    }

    private async Task EnsurePasswordIsValidAsync(AuthUser user, string password)
    {
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);

        if (!isPasswordValid)
        {
            // TODO: Create custom exception for invalid login.
            throw new UnauthorizedAccessException();
        }
    }
}