using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<IdentityResult> UpdatePassword(User user, string currentPassword, string newPassword);
        Task<IdentityResult> UpdateEmail(User user, string newEmail);
        Task<IdentityResult> UpdateUsername(User user, string newUsername);
    }
}