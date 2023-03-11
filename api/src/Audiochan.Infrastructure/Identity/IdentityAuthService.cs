using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core;
using Audiochan.Core.Entities;
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

    public IdentityAuthService(UserManager<AudiochanIdentityUser> userManager, ITokenProvider tokenProvider, IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _userManager = userManager;
        _tokenProvider = tokenProvider;
        _dbContextFactory = dbContextFactory;
    }

    public async Task<LoginResult> LoginWithPasswordAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var user = await _userManager.FindByNameAsync(login);

        if (user is null)
        {
            return new LoginWithPasswordFailed();
        }

        var appUser = await dbContext.Users.FirstOrDefaultAsync(x => x.IdentityId == user.Id, cancellationToken);

        await EnsurePasswordIsValidAsync(user, password);

        var claims = GetClaims(user, appUser);
        var token = _tokenProvider.GenerateAccessToken(claims, DateTime.UtcNow.AddMinutes(30));

        return new AuthTokenResult(token);
    }

    public Task RevokeRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default)
    {
        // TODO: Add implementation for revoking refresh token.
        return Task.CompletedTask;
    }

    private IEnumerable<Claim> GetClaims(AudiochanIdentityUser user, User? appUser)
    {
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

        return claims;
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