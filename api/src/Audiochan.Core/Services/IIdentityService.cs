using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Services
{
    public interface IIdentityService
    {
        Task<List<string>> GetRolesAsync(User user);
        Task<User?> LoginUserAsync(string login, string password, CancellationToken cancellationToken = default);
        Task<Result<bool>> CreateUser(User user, string password);
        Task<Result<bool>> UpdatePassword(User user, string currentPassword, string newPassword);
        Task<Result<bool>> UpdateEmail(User user, string newEmail);
        Task<Result<bool>> UpdateUsername(User user, string newUsername);
    }
}