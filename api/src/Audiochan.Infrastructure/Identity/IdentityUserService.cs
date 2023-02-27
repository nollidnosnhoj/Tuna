using System;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Features.Auth;
using Audiochan.Core.Features.Auth.Models;
using Audiochan.Infrastructure.Identity.Models;
using Audiochan.Infrastructure.Identity.Extensions;
using Microsoft.AspNetCore.Identity;
using IdentityResult = Audiochan.Core.Features.Auth.Models.IdentityResult;
using IdentityUser = Audiochan.Core.Features.Auth.Models.IdentityUser;

namespace Audiochan.Infrastructure.Identity;

public class IdentityUserService : IIdentityService
{
    private readonly UserManager<IdUser> _userManager;

    public IdentityUserService(UserManager<IdUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityUser?> GetUserAsync(string id, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return null;
        return new IdentityUser
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName
        };
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
            throw new UnauthorizedAccessException();
        }
        
        var identityResult = await _userManager.SetUserNameAsync(user, userName);

        return identityResult.ToAppIdentityResult();
    }

    public async Task<IdentityResult> UpdateEmailAsync(string identityId, string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(identityId);
        
        if (user is null)
        {
            throw new UnauthorizedAccessException();
        }
        
        var identityResult = await _userManager.SetEmailAsync(user, email);

        return identityResult.ToAppIdentityResult();
    }

    public async Task<IdentityResult> UpdatePasswordAsync(string identityId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(identityId);
        
        if (user is null)
        {
            throw new UnauthorizedAccessException();
        }

        var identityResult = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);

        return identityResult.ToAppIdentityResult();
    }
}