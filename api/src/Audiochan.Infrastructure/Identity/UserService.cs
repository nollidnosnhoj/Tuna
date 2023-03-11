using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Features.Auth;
using Audiochan.Core.Features.Auth.Models;
using Audiochan.Core.Services;
using Audiochan.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Infrastructure.Identity;

public class UserService : IIdentityService
{
    private readonly UserManager<AudiochanIdentityUser> _userManager;

    public UserService(UserManager<AudiochanIdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IdentityUserDto?> GetUserAsync(string id, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return null;
        return new IdentityUserDto
        {
            Id = user.Id,
            Email = user.Email,
            UserName = user.UserName
        };
    }

    public async Task<NewUserIdentityResult> CreateUserAsync(string userName, string email, string password, CancellationToken cancellationToken = default)
    {
        var user = new AudiochanIdentityUser(userName)
        {
            Email = email
        };
        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return (NewUserIdentityResult)IdentityResult.Failed(result.Errors.ToArray());
        }

        return NewUserIdentityResult.Success(user.Id);
    }

    public async Task<IdentityResult> UpdateUserNameAsync(string identityId, string userName, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(identityId);
            
        if (user is null)
        {
            throw new UnauthorizedAccessException();
        }
        
        return await _userManager.SetUserNameAsync(user, userName);
    }

    public async Task<IdentityResult> UpdateEmailAsync(string identityId, string email, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(identityId);
        
        if (user is null)
        {
            throw new UnauthorizedAccessException();
        }
        
        return await _userManager.SetEmailAsync(user, email);
    }

    public async Task<IdentityResult> UpdatePasswordAsync(string identityId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await _userManager.FindByIdAsync(identityId);
        
        if (user is null)
        {
            throw new UnauthorizedAccessException();
        }

        return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
    }
}