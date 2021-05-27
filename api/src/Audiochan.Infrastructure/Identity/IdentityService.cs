using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
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

        public async Task<IdentityResult> UpdatePassword(User user, string currentPassword, string newPassword)
        {
            return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        }

        public async Task<IdentityResult> UpdateEmail(User user, string newEmail)
        {
            var result = await _userManager.SetEmailAsync(user, newEmail);
            
            if (result.Succeeded)
            {
                await _userManager.UpdateNormalizedEmailAsync(user);
            }

            return result;
        }

        public async Task<IdentityResult> UpdateUsername(User user, string newUsername)
        {
            var result = await _userManager.SetUserNameAsync(user, newUsername);
            
            if (result.Succeeded)
            {
                await _userManager.UpdateNormalizedUserNameAsync(user);
                user.DisplayName = user.UserName;
                await _userManager.UpdateAsync(user);
            }

            return result;
        }
    }
}