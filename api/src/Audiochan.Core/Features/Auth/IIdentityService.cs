using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Features.Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace Audiochan.Core.Features.Auth;

public interface IIdentityService
{
    Task<IdentityUser?> GetUserAsync(string id, CancellationToken cancellationToken = default);
    Task<NewUserIdentityResult> CreateUserAsync(string userName, string email, string password, CancellationToken cancellationToken = default);
    Task<IdentityResult> UpdateUserNameAsync(string identityId, string userName, CancellationToken cancellationToken = default);
    Task<IdentityResult> UpdateEmailAsync(string identityId, string email, CancellationToken cancellationToken = default);
    Task<IdentityResult> UpdatePasswordAsync(string identityId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
}