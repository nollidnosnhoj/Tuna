using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Exceptions;
using Audiochan.Core.Features.Auth;
using Audiochan.Core.Features.Auth.Models;
using Audiochan.Core.Persistence;
using Audiochan.Core.Security;
using Audiochan.Domain.Entities;
using Audiochan.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;

namespace Audiochan.Infrastructure.Identity;

public class IdentityAuthService : IAuthService
{
    private readonly UserManager<IdUser> _userManager;
    private readonly ITokenService _tokenService;
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public IdentityAuthService(UserManager<IdUser> userManager, ITokenService tokenService, IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _userManager = userManager;
        _tokenService = tokenService;
        _dbContextFactory = dbContextFactory;
    }

    public async Task<LoginResult> LoginWithPasswordAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var user = await _userManager.FindByNameAsync(login);

        if (user is null)
        {
            // TODO: Create custom exception for invalid login.
            throw new UnauthorizedException();
        }

        var appUser = await dbContext.Users.FirstOrDefaultAsync(x => x.IdentityId == user.Id, cancellationToken);

        await EnsurePasswordIsValidAsync(user, password);

        var claims = GetClaims(user, appUser);
        var token = _tokenService.GenerateAccessToken(claims);

        return new LoginResult(token);
    }

    private IEnumerable<Claim> GetClaims(IdUser user, User? appUser)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new("hasProfile", (appUser is not null).ToString())
        };
        
        if (appUser is not null)
        {
            claims.Add(new Claim("userId", appUser.Id.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Name, appUser.UserName));
        }

        return claims;
    }

    private async Task EnsurePasswordIsValidAsync(IdUser user, string password)
    {
        var isPasswordValid = await _userManager.CheckPasswordAsync(user, password);

        if (!isPasswordValid)
        {
            // TODO: Create custom exception for invalid login.
            throw new UnauthorizedException();
        }
    }
}