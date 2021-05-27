using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Models;

namespace Audiochan.Core.Services
{
    public interface IIdentityService
    {
        Task<Result<bool>> CreateUser(User user, string password);
        Task<Result<bool>> UpdatePassword(User user, string currentPassword, string newPassword);
        Task<Result<bool>> UpdateEmail(User user, string newEmail);
        Task<Result<bool>> UpdateUsername(User user, string newUsername);
    }
}