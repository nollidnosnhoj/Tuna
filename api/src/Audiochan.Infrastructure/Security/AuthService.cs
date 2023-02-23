using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Dtos;
using Audiochan.Common.Exceptions;
using Audiochan.Core.Services;
using Audiochan.Infrastructure.Security.Models;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Infrastructure.Security;

public class AuthService : IAuthService
{
    private readonly UserManager<IdUser> _userManager;

    public AuthService(UserManager<IdUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<LoginResult> LoginWithPasswordAsync(string login, string password, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByNameAsync(login);

        if (user is null)
        {
            // TODO: Create custom exception for invalid login.
            throw new UnauthorizedException();
        }

        await EnsurePasswordIsValidAsync(user, password);

        var token = await GenerateTokenAsync(user, cancellationToken);

        return new LoginResult(token);
    }

    private Task<string> GenerateTokenAsync(IdUser user, CancellationToken cancellationToken = default)
    {
        // TODO: Generate JWT
        var token = string.Empty;
        return Task.FromResult(token);
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