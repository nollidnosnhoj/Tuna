using System.Threading;
using System.Threading.Tasks;
using Audiochan.Common.Dtos;
using Audiochan.Common.Exceptions;
using Audiochan.Core.Services;
using Audiochan.Infrastructure.Security.Extensions;
using Audiochan.Infrastructure.Security.Models;
using Microsoft.AspNetCore.Identity;
using IdentityResult = Audiochan.Common.Dtos.IdentityResult;

namespace Audiochan.Infrastructure.Security;

public class IdentityService : IIdentityService
{
    private readonly UserManager<IdUser> _userManager;

    public IdentityService(UserManager<IdUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<NewUserIdentityResult> CreateUserAsync(string userName, string email, string password, CancellationToken cancellationToken = default)
    {
        var user = new IdUser(userName)
        {
            Email = email
        };
        var result = await _userManager.CreateAsync(user, password);

        return new NewUserIdentityResult(
            result.Succeeded,
            user.Id,
            result.Errors.ToAppIdentityError());
    }

    public async Task<IdentityResult> UpdateUserNameAsync(string identityId, string userName, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(identityId);
            
        if (user is null)
        {
            throw new UnauthorizedException();
        }
        
        var identityResult = await _userManager.SetUserNameAsync(user, userName);

        return identityResult.ToAppIdentityResult();
    }

    public async Task<IdentityResult> UpdateEmailAsync(string identityId, string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(identityId);
        
        if (user is null)
        {
            throw new UnauthorizedException();
        }
        
        var identityResult = await _userManager.SetEmailAsync(user, email);

        return identityResult.ToAppIdentityResult();
    }

    public async Task<IdentityResult> UpdatePasswordAsync(string identityId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(identityId);
        
        if (user is null)
        {
            throw new UnauthorizedException();
        }

        var identityResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        return identityResult.ToAppIdentityResult();
    }
}