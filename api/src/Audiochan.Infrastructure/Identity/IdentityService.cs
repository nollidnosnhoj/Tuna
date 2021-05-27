using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Models;
using Audiochan.Core.Services;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Infrastructure.Identity
{
    internal class IdentityService : IIdentityService
    {
        private readonly UserManager<User> _userManager;

        public IdentityService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<Result<bool>> CreateUser(User user, string password)
        {
            return (await _userManager.CreateAsync(user, password)).ToResult();
        }

        public async Task<Result<bool>> UpdatePassword(User user, string currentPassword, string newPassword)
        {
            return (await _userManager.ChangePasswordAsync(user, currentPassword, newPassword)).ToResult();
        }

        public async Task<Result<bool>> UpdateEmail(User user, string newEmail)
        {
            var result = await _userManager.SetEmailAsync(user, newEmail);
            
            if (result.Succeeded)
            {
                await _userManager.UpdateNormalizedEmailAsync(user);
            }

            return result.ToResult();
        }

        public async Task<Result<bool>> UpdateUsername(User user, string newUsername)
        {
            var result = await _userManager.SetUserNameAsync(user, newUsername);
            
            if (result.Succeeded)
            {
                await _userManager.UpdateNormalizedUserNameAsync(user);
                user.DisplayName = user.UserName;
                await _userManager.UpdateAsync(user);
            }

            return result.ToResult();
        }
    }
}